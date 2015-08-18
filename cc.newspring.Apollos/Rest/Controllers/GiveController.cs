﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Rock;
using Rock.Data;
using Rock.Financial;
using Rock.Model;
using Rock.Rest;
using Rock.Rest.Filters;
using Rock.Web.Cache;

namespace cc.newspring.Apollos.Rest.Controllers
{
    public class GiveController : ApiControllerBase
    {
        private static string gatewayName = "cc.newspring.CyberSource.Gateway";

        /// <summary>
        /// Schedules the giving.
        /// </summary>
        /// <returns></returns>
        [Authenticate, Secured]
        [HttpPost]
        [System.Web.Http.Route( "api/ScheduleGiving/Stop/{id}" )]
        public HttpResponseMessage StopScheduledGiving( int id )
        {
            var rockContext = new RockContext();
            var gatewayComponent = GatewayContainer.GetComponent( gatewayName );

            if ( gatewayComponent == null )
            {
                return GenerateResponse( HttpStatusCode.InternalServerError, "There was a problem creating the gateway component" );
            }

            var financialGateway = new FinancialGatewayService( rockContext ).Queryable().FirstOrDefault( g => g.EntityTypeId == gatewayComponent.EntityType.Id );

            if ( financialGateway == null )
            {
                return GenerateResponse( HttpStatusCode.InternalServerError, "There was a problem creating the financial gateway" );
            }

            var schedule = ( new FinancialScheduledTransactionService( rockContext ) ).Get( id );

            if ( schedule == null )
            {
                return GenerateResponse( HttpStatusCode.BadRequest, "No schedule with Id: " + id );
            }

            string errorMessage;
            var error = gatewayComponent.CancelScheduledPayment(schedule, out errorMessage);

            if ( error || errorMessage != null )
            {
                return GenerateResponse( HttpStatusCode.InternalServerError, errorMessage ?? "There was an error with the gateway" );
            }

            schedule.IsActive = false;
            rockContext.SaveChanges();
            return GenerateResponse( HttpStatusCode.BadRequest, "Stopping " + id );  
        }

        /// <summary>
        /// Schedules the giving.
        /// </summary>
        /// <returns></returns>
        [Authenticate, Secured]
        [HttpPost]
        [System.Web.Http.Route( "api/ScheduleGiving" )]
        public HttpResponseMessage ScheduleGiving( [FromBody]ScheduleParameters scheduleParameters )
        {
            var errorResponse = ValidateGiveParams( scheduleParameters );

            if ( errorResponse != null )
            {
                return errorResponse;
            }

            var rockContext = new RockContext();

            if ( !scheduleParameters.StartDate.HasValue )
            {
                return GenerateResponse( HttpStatusCode.BadRequest, "Schedule must contain a StartDate" );
            }

            var timeSpan = DateTime.Now - scheduleParameters.StartDate.Value;

            if ( timeSpan > TimeSpan.FromDays(0) )
            {
                return GenerateResponse( HttpStatusCode.BadRequest, "Schedule must contain a StartDate that occurs in the future" );
            }

            var paymentSchedule = new PaymentSchedule();

            paymentSchedule.StartDate = scheduleParameters.StartDate.Value;

            if ( !scheduleParameters.FrequencyValueGuid.HasValue )
            {
                return GenerateResponse( HttpStatusCode.BadRequest, "Schedule must contain a FrequencyValueGuid" );
            }

            paymentSchedule.TransactionFrequencyValue = DefinedValueCache.Read( scheduleParameters.FrequencyValueGuid.Value );

            if ( paymentSchedule.TransactionFrequencyValue == null )
            {
                return GenerateResponse( HttpStatusCode.BadRequest, "Schedule must contain a valid FrequencyValueGuid" );
            }

            var gatewayComponent = GatewayContainer.GetComponent( gatewayName );

            if ( gatewayComponent == null )
            {
                return GenerateResponse( HttpStatusCode.InternalServerError, "There was a problem creating the gateway component" );
            }

            var financialGateway = new FinancialGatewayService( rockContext ).Queryable().FirstOrDefault( g => g.EntityTypeId == gatewayComponent.EntityType.Id );

            if ( financialGateway == null )
            {
                return GenerateResponse( HttpStatusCode.InternalServerError, "There was a problem creating the financial gateway" );
            }

            var person = GetGiverPerson( scheduleParameters, rockContext, out errorResponse );

            if ( errorResponse != null )
            {
                return errorResponse;
            }

            if ( person == null )
            {
                return GenerateResponse( HttpStatusCode.InternalServerError, "There was a problem creating the payment info" );
            }

            paymentSchedule.PersonId = person.Id;
            var totalAmount = CalculateTotalAmount( scheduleParameters, rockContext, out errorResponse );

            if ( errorResponse != null )
            {
                return errorResponse;
            }

            if ( !totalAmount.HasValue || totalAmount.Value <= 0 )
            {
                return GenerateResponse( HttpStatusCode.InternalServerError, "There was a problem calculating the total amount" );
            }

            var paymentInfo = GetPaymentInfo( scheduleParameters, rockContext, person.PrimaryAliasId, totalAmount.Value, out errorResponse );

            if ( errorResponse != null )
            {
                return errorResponse;
            }

            if ( paymentInfo == null )
            {
                return GenerateResponse( HttpStatusCode.InternalServerError, "There was a problem creating the payment info" );
            }

            string errorMessage;
            var schedule = gatewayComponent.AddScheduledPayment(financialGateway, paymentSchedule, paymentInfo, out errorMessage);

            if ( schedule == null || !string.IsNullOrWhiteSpace( errorMessage ) )
            {
                return GenerateResponse( HttpStatusCode.InternalServerError, errorMessage ?? "The gateway had a problem and/or did not create a transaction as expected" );
            }

            schedule.TransactionFrequencyValueId = paymentSchedule.TransactionFrequencyValue.Id;

            if ( person.PrimaryAliasId.HasValue )
            {
                schedule.AuthorizedPersonAliasId = person.PrimaryAliasId.Value;
            }

            schedule.FinancialPaymentDetail = new FinancialPaymentDetail {
                CurrencyTypeValueId = paymentInfo.CurrencyTypeValue.Id
            };

            if ( paymentInfo.CreditCardTypeValue != null )
            {
                schedule.FinancialPaymentDetail.CreditCardTypeValueId = paymentInfo.CreditCardTypeValue.Id;
            }

            foreach ( var accountAmount in scheduleParameters.AmountDetails )
            {
                schedule.ScheduledTransactionDetails.Add( new FinancialScheduledTransactionDetail()
                {
                    Amount = accountAmount.Amount,
                    AccountId = accountAmount.TargetAccountId
                } );
            }

            new FinancialScheduledTransactionService( rockContext ).Add( schedule );
            rockContext.SaveChanges();
            return GenerateResponse( HttpStatusCode.NoContent );
        }

        /// <summary>
        /// Gives through CyberSource with the specified give parameters.
        /// </summary>
        /// <param name="giveParameters">The give parameters.</param>
        /// <returns></returns>
        [Authenticate, Secured]
        [HttpPost]
        [System.Web.Http.Route( "api/Give" )]
        public HttpResponseMessage Give( [FromBody]GiveParameters giveParameters )
        {
            var errorResponse = ValidateGiveParams( giveParameters );

            if ( errorResponse != null )
            {
                return errorResponse;
            }

            var rockContext = new RockContext();
            decimal? totalAmount = 0m;

            totalAmount = CalculateTotalAmount(giveParameters, rockContext, out errorResponse);

            if ( errorResponse != null )
            {
                return errorResponse;
            }

            if ( !totalAmount.HasValue || totalAmount.Value <= 0 )
            {
                return GenerateResponse( HttpStatusCode.InternalServerError, "There was a problem calculating the total amount" );
            }

            var gatewayComponent = GatewayContainer.GetComponent( gatewayName );

            if ( gatewayComponent == null )
            {
                return GenerateResponse( HttpStatusCode.InternalServerError, "There was a problem creating the gateway component" );
            }

            var financialGateway = new FinancialGatewayService( rockContext ).Queryable().FirstOrDefault( g => g.EntityTypeId == gatewayComponent.EntityType.Id );

            if ( financialGateway == null )
            {
                return GenerateResponse( HttpStatusCode.InternalServerError, "There was a problem creating the financial gateway" );
            }
            
            var person = GetGiverPerson( giveParameters, rockContext, out errorResponse );

            if ( errorResponse != null )
            {
                return errorResponse;
            }

            if ( person == null )
            {
                return GenerateResponse( HttpStatusCode.InternalServerError, "There was a problem creating the person" );
            }
                        
            var paymentInfo = GetPaymentInfo(giveParameters, rockContext, person.PrimaryAliasId, totalAmount.Value, out errorResponse);

            if ( errorResponse != null )
            {
                return errorResponse;
            }

            if ( paymentInfo == null )
            {
                return GenerateResponse( HttpStatusCode.InternalServerError, "There was a problem creating the payment info" );
            }

            string errorMessage;
            var transaction = gatewayComponent.Charge( financialGateway, paymentInfo, out errorMessage );

            if ( transaction == null || !string.IsNullOrWhiteSpace( errorMessage ) )
            {
                return GenerateResponse( HttpStatusCode.InternalServerError, errorMessage ?? "The gateway had a problem and/or did not create a transaction as expected" );
            }

            transaction.TransactionDateTime = RockDateTime.Now;
            transaction.AuthorizedPersonAliasId = person.PrimaryAliasId;
            transaction.AuthorizedPersonAlias = person.PrimaryAlias;
            transaction.FinancialGatewayId = financialGateway.Id;
            transaction.TransactionTypeValueId = DefinedValueCache.Read( new Guid( Rock.SystemGuid.DefinedValue.TRANSACTION_TYPE_CONTRIBUTION ) ).Id;

            transaction.FinancialPaymentDetail = new FinancialPaymentDetail {
                CurrencyTypeValueId = paymentInfo.CurrencyTypeValue.Id
            };
            var detail = transaction.FinancialPaymentDetail;

            if ( paymentInfo.CreditCardTypeValue != null )
            {
                detail.CreditCardTypeValueId = paymentInfo.CreditCardTypeValue.Id;
            }

            foreach ( var accountAmount in giveParameters.AmountDetails )
            {
                transaction.TransactionDetails.Add( new FinancialTransactionDetail()
                {
                    Amount = accountAmount.Amount,
                    AccountId = accountAmount.TargetAccountId
                } );
            }

            new FinancialTransactionService( rockContext ).Add( transaction );
            rockContext.SaveChanges();

            if ( !giveParameters.SourceAccountId.HasValue )
            {
                // Get a reference number to allow "saving" this account for future use (don't throw errors here because the gateway already received the payment request)
                var newReferenceNumber = gatewayComponent.GetReferenceNumber( transaction, out errorMessage );

                // If we got a reference number and we can reference the person, save the account - skip if there was an error getting the reference number
                if ( person.PrimaryAliasId.HasValue && !string.IsNullOrWhiteSpace( newReferenceNumber ) && string.IsNullOrWhiteSpace( errorMessage ) )
                {
                    var savedAccountService = new FinancialPersonSavedAccountService( rockContext );
                    var maskedAccountNumber = Mask( giveParameters.AccountNumber );

                    // Check for an account belonging to this person with the same mask that already has a reference
                    var savedAccount = savedAccountService.Queryable().Where( a =>
                        a.PersonAliasId == person.PrimaryAliasId.Value &&
                        a.FinancialPaymentDetail.AccountNumberMasked == maskedAccountNumber &&
                        a.ReferenceNumber != null ).FirstOrDefault();

                    // If that account does not exist, save this account for future giving ease
                    if ( savedAccount == null )
                    {
                        savedAccount = new FinancialPersonSavedAccount();
                        savedAccount.FinancialPaymentDetailId = transaction.FinancialPaymentDetailId;
                        savedAccount.PersonAliasId = person.PrimaryAliasId;
                        savedAccount.TransactionCode = transaction.TransactionCode;
                        savedAccount.FinancialGatewayId = financialGateway.Id;
                        savedAccount.ReferenceNumber = newReferenceNumber;
                        transaction.FinancialPaymentDetail.AccountNumberMasked = maskedAccountNumber;                        

                        if ( paymentInfo.CreditCardTypeValue != null )
                        {
                            savedAccount.Name = paymentInfo.CreditCardTypeValue.Description;
                        }
                        else
                        {
                            var name = giveParameters.AccountType;
                            savedAccount.Name = char.ToUpper( name[0] ) + name.Substring( 1 );
                        }

                        savedAccountService.Add( savedAccount );
                    }

                    // If this is a bank account, save it to the bank account service for check scanning functionality
                    if ( giveParameters.AccountType.ToLower() == "checking" || giveParameters.AccountType.ToLower() == "savings" )
                    {
                        var bankAccountService = new FinancialPersonBankAccountService( rockContext );
                        var accountNumberSecured = FinancialPersonBankAccount.EncodeAccountNumber( giveParameters.RoutingNumber, giveParameters.AccountNumber );
                        var bankAccount = bankAccountService.Queryable().Where( a =>
                            a.AccountNumberSecured == accountNumberSecured &&
                            a.PersonAliasId == person.PrimaryAliasId.Value ).FirstOrDefault();

                        if ( bankAccount == null )
                        {
                            bankAccount = new FinancialPersonBankAccount();
                            bankAccount.PersonAliasId = person.PrimaryAliasId.Value;
                            bankAccount.AccountNumberMasked = maskedAccountNumber;
                            bankAccount.AccountNumberSecured = accountNumberSecured;
                            bankAccountService.Add( bankAccount );
                        }
                    }
                }
            }

            rockContext.SaveChanges();
            return GenerateResponse( HttpStatusCode.NoContent );
        }

        /// <summary>
        /// Creates a person within his/her own family using the giving parameters.
        /// </summary>
        /// <param name="giveParameters">The give parameters.</param>
        /// <returns></returns>
        private Person CreatePerson( GiveParameters giveParameters, RockContext rockContext )
        {
            var person = new Person()
            {
                FirstName = giveParameters.FirstName,
                LastName = giveParameters.LastName,
                IsEmailActive = true,
                Email = giveParameters.Email,
                EmailPreference = EmailPreference.EmailAllowed,
                RecordTypeValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id,
                ConnectionStatusValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_CONNECTION_STATUS_PARTICIPANT ).Id,
                RecordStatusValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE ).Id
            };

            // Create Person/Family
            var familyGroup = PersonService.SaveNewPerson( person, rockContext, giveParameters.CampusId, false );

            if ( !string.IsNullOrWhiteSpace( giveParameters.PhoneNumber ) )
            {
                person.PhoneNumbers.Add( new PhoneNumber()
                {
                    Number = giveParameters.PhoneNumber,
                    NumberTypeValueId = DefinedValueCache.Read( new Guid( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME ) ).Id
                } );
            }

            if ( !( string.IsNullOrWhiteSpace( giveParameters.Street1 ) ||
                        string.IsNullOrWhiteSpace( giveParameters.City ) ||
                        string.IsNullOrWhiteSpace( giveParameters.State ) ||
                        string.IsNullOrWhiteSpace( giveParameters.PostalCode ) ) )
            {
                familyGroup.GroupLocations.Add( new GroupLocation()
                {
                    GroupLocationTypeValueId = DefinedValueCache.Read( new Guid( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME ) ).Id,
                    Location = new Location()
                    {
                        Street1 = giveParameters.Street1,
                        Street2 = giveParameters.Street2,
                        City = giveParameters.City,
                        State = giveParameters.State,
                        PostalCode = giveParameters.PostalCode,
                        Country = giveParameters.Country
                    }
                } );
            }

            rockContext.SaveChanges();
            return person;
        }

        /// <summary>
        /// Generates a response for an API request.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        private HttpResponseMessage GenerateResponse( HttpStatusCode code, string message = null )
        {
            var response = new HttpResponseMessage( code );

            if ( !string.IsNullOrWhiteSpace( message ) )
            {
                response.Content = new StringContent( message );
            }

            return response;
        }

        /// <summary>
        /// Masks the specified string to look something like ************1234.
        /// </summary>
        /// <param name="unmasked">The unmasked string.</param>
        /// <param name="charsToShow">The number of chars to show.</param>
        /// <param name="maskChar">The mask character.</param>
        /// <returns></returns>
        private string Mask( string unmasked, int charsToShow = 4, char maskChar = '*' )
        {
            var lengthOfUnmasked = unmasked.Length;
            var charsToMask = lengthOfUnmasked - charsToShow;

            if ( lengthOfUnmasked <= charsToShow )
            {
                return unmasked;
            }

            var mask = string.Empty.PadLeft( charsToMask, maskChar );
            var shown = unmasked.Substring( unmasked.Length - charsToShow );
            return string.Concat( mask, shown );
        }

        private PaymentInfo GetPaymentInfo( GiveParameters giveParameters, RockContext rockContext, int? personPrimaryAliasId, decimal totalAmount, out HttpResponseMessage errorResponse )
        {
            errorResponse = null;
            PaymentInfo paymentInfo = null;

            if ( giveParameters.SourceAccountId.HasValue )
            {
                var account = new FinancialPersonSavedAccountService( rockContext ).Get( giveParameters.SourceAccountId.Value );

                if ( account != null && account.PersonAliasId.HasValue && account.PersonAliasId == personPrimaryAliasId )
                {
                    paymentInfo = account.GetReferencePayment();
                }
                else
                {
                    errorResponse = GenerateResponse( HttpStatusCode.BadRequest, "The SourceAccountId did not resolve to a saved account's id" );
                    return null;
                }
            }
            else
            {
                if ( giveParameters.AccountType.ToLower() == "credit" )
                {
                    paymentInfo = new CreditCardPaymentInfo()
                    {
                        Number = giveParameters.AccountNumber,
                        Code = giveParameters.CCV,
                        ExpirationDate = new DateTime( giveParameters.ExpirationYear, giveParameters.ExpirationMonth, 1 ),
                        BillingStreet1 = giveParameters.Street1,
                        BillingStreet2 = giveParameters.Street2,
                        BillingCity = giveParameters.City,
                        BillingState = giveParameters.State,
                        BillingPostalCode = giveParameters.PostalCode,
                        BillingCountry = giveParameters.Country
                    };
                }
                else
                {
                    paymentInfo = new ACHPaymentInfo()
                    {
                        BankRoutingNumber = giveParameters.RoutingNumber,
                        BankAccountNumber = giveParameters.AccountNumber,
                        AccountType = giveParameters.AccountType.ToLower() == "checking" ? BankAccountType.Checking : BankAccountType.Savings
                    };
                }
            }

            paymentInfo.Amount = totalAmount;
            paymentInfo.FirstName = giveParameters.FirstName;
            paymentInfo.LastName = giveParameters.LastName;
            paymentInfo.Email = giveParameters.Email;
            paymentInfo.Phone = giveParameters.PhoneNumber;
            paymentInfo.Street1 = giveParameters.Street1;
            paymentInfo.Street2 = giveParameters.Street2;
            paymentInfo.City = giveParameters.City;
            paymentInfo.State = giveParameters.State;
            paymentInfo.PostalCode = giveParameters.PostalCode;
            paymentInfo.Country = giveParameters.Country;

            return paymentInfo;
        }

        private Person GetGiverPerson( GiveParameters giveParameters, RockContext rockContext, out HttpResponseMessage errorResponse )
        {
            errorResponse = null;
            Person person = null;

            if ( giveParameters.PersonId.HasValue )
            {
                person = new PersonService( rockContext ).Get( giveParameters.PersonId.Value );

                if ( person == null )
                {
                    errorResponse = GenerateResponse( HttpStatusCode.BadRequest, "The PersonId did not resolve to an existing person record" );
                    return null;
                }

                if ( string.IsNullOrWhiteSpace( person.Email ) )
                {
                    person.Email = giveParameters.Email;
                }

                if ( !person.PhoneNumbers.Any() && !string.IsNullOrWhiteSpace( giveParameters.PhoneNumber ) )
                {
                    person.PhoneNumbers.Add( new PhoneNumber()
                    {
                        Number = giveParameters.PhoneNumber,
                        NumberTypeValueId = DefinedValueCache.Read( new Guid( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME ) ).Id
                    } );
                }
            }
            else
            {
                if ( !giveParameters.CampusId.HasValue )
                {
                    errorResponse = GenerateResponse( HttpStatusCode.BadRequest, "CampusId is required when creating a new person" );
                    return null;
                }

                if ( new CampusService( rockContext ).Get( giveParameters.CampusId.Value ) == null )
                {
                    errorResponse = GenerateResponse( HttpStatusCode.BadRequest, "CampusId must be an existing campus's id" );
                    return null;
                }

                person = CreatePerson( giveParameters, rockContext );

                if ( person == null )
                {
                    errorResponse = GenerateResponse( HttpStatusCode.InternalServerError, "There was a problem creating a person with the supplied details" );
                    return null;
                }
            }

            return person;
        }

        private decimal? CalculateTotalAmount( GiveParameters giveParameters, RockContext rockContext, out HttpResponseMessage errorResponse )
        {
            var totalAmount = 0m;
            errorResponse = null;

            foreach ( var accountAmount in giveParameters.AmountDetails )
            {
                if ( accountAmount.Amount < 1m )
                {
                    errorResponse = GenerateResponse( HttpStatusCode.BadRequest, "AmountDetails/Amount is required and must be greater than or equal to 1" );
                    return null;
                }
                if ( accountAmount.TargetAccountId == 0 )
                {
                    errorResponse = GenerateResponse( HttpStatusCode.BadRequest, "AmountDetails/TargetAccountId is required" );
                    return null;
                }
                if ( new FinancialAccountService( rockContext ).Get( accountAmount.TargetAccountId ) == null )
                {
                    errorResponse = GenerateResponse( HttpStatusCode.BadRequest, "AmountDetails/TargetAccountId must be an existing account's id" );
                    return null;
                }

                totalAmount += accountAmount.Amount;
            }

            return totalAmount;
        }

        private HttpResponseMessage ValidateGiveParams(GiveParameters giveParameters)
        {
            // Non-required fields should be empty strings to prevent null reference exceptions in the gateway
            giveParameters.PhoneNumber = giveParameters.PhoneNumber ?? string.Empty;

            // Validate required fields for all cases
            if ( string.IsNullOrWhiteSpace( giveParameters.Email ) )
            {
                return GenerateResponse( HttpStatusCode.BadRequest, "Email is required" );
            }

            if ( string.IsNullOrWhiteSpace( giveParameters.FirstName ) || string.IsNullOrWhiteSpace( giveParameters.LastName ) )
            {
                return GenerateResponse( HttpStatusCode.BadRequest, "FirstName and LastName are required" );
            }

            if ( giveParameters.State != null && giveParameters.State.Length != 2 )
            {
                return GenerateResponse( HttpStatusCode.BadRequest, "State must be a 2 letter string" );
            }

            if ( giveParameters.SourceAccountId.HasValue )
            {
                // Validate saved account case
                if ( !giveParameters.PersonId.HasValue )
                {
                    return GenerateResponse( HttpStatusCode.BadRequest, "PersonId is required to use an existing account (SourceAccountId)" );
                }
            }
            else
            {
                // No existing account case
                if ( string.IsNullOrWhiteSpace( giveParameters.AccountNumber ) && !giveParameters.SourceAccountId.HasValue )
                {
                    return GenerateResponse( HttpStatusCode.BadRequest, "AccountNumber is required" );
                }

                if ( string.IsNullOrWhiteSpace( giveParameters.AccountType ) && !giveParameters.SourceAccountId.HasValue )
                {
                    return GenerateResponse( HttpStatusCode.BadRequest, "AccountType is required and must be one of checking, savings, or credit" );
                }

                switch ( giveParameters.AccountType.ToLower() )
                {
                    case "checking":
                    case "savings":
                        if ( string.IsNullOrWhiteSpace( giveParameters.RoutingNumber ) )
                        {
                            return GenerateResponse( HttpStatusCode.BadRequest, "RoutingNumber is required for ACH transactions" );
                        }
                        break;

                    case "credit":
                        // Non-required fields should be empty strings to prevent null reference exceptions in the gateway
                        giveParameters.CCV = giveParameters.CCV ?? string.Empty;

                        if ( giveParameters.ExpirationMonth < 1 || giveParameters.ExpirationMonth > 12 )
                        {
                            return GenerateResponse( HttpStatusCode.BadRequest, "ExpirationMonth is required and must be between 1 and 12 for credit transactions" );
                        }

                        var currentDate = DateTime.Now;
                        var maxYear = currentDate.Year + 30;

                        if ( giveParameters.ExpirationYear < currentDate.Year || giveParameters.ExpirationYear > maxYear )
                        {
                            return GenerateResponse( HttpStatusCode.BadRequest, string.Format( "ExpirationYear is required and must be between {0} and {1} for credit transactions", currentDate.Year, maxYear ) );
                        }

                        if ( giveParameters.ExpirationYear <= currentDate.Year && giveParameters.ExpirationMonth < currentDate.Month )
                        {
                            return GenerateResponse( HttpStatusCode.BadRequest, "The ExpirationMonth and ExpirationYear combination must not have already elapsed for credit transactions" );
                        }

                        if ( string.IsNullOrWhiteSpace( giveParameters.Street1 ) ||
                            string.IsNullOrWhiteSpace( giveParameters.City ) ||
                            string.IsNullOrWhiteSpace( giveParameters.State ) ||
                            string.IsNullOrWhiteSpace( giveParameters.PostalCode ) )
                        {
                            return GenerateResponse( HttpStatusCode.BadRequest, "Street1, City, State, and PostalCode are required for credit transactions" );
                        }

                        break;

                    default:
                        return GenerateResponse( HttpStatusCode.BadRequest, "AccountType is required and must be one of checking, savings, or credit" );
                }
            }

            if ( giveParameters.AmountDetails == null || giveParameters.AmountDetails.Length == 0 )
            {
                return GenerateResponse( HttpStatusCode.BadRequest, "AmountDetails are required and the sum of them must be greater than or equal to 1" );
            }

            return null;
        }
    }
}