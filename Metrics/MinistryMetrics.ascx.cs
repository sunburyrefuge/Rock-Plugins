﻿// <copyright>
// Copyright 2015 by NewSpring Church
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using Newtonsoft.Json;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.cc_newspring.Blocks.Metrics
{
    /// <summary>
    /// All Church Metrics Block
    /// </summary>
    [DisplayName( "Ministry Metrics" )]
    [Category( "NewSpring" )]
    [Description( "Custom church metrics block using the Chart.js library" )]
    [CustomDropdownListField( "Number of Columns", "", "1,2,3,4,5,6,7,8,9,10,11,12", false, DefaultValue = "12", Order = 1 )]
    [CustomDropdownListField( "Metric Display Type", "", "Text,Line,Donut", false, "Text", Order = 2 )]
    [TextField( "Primary Metric Key", "If this is used, do not select a primary metric source", Order = 3 )]
    [MetricCategoriesField( "Primary Metric Source", "Select the primary metric to include in this chart.", Order = 4 )]
    [CustomRadioListField( "Metric Comparison", "Is this metric a sum of the selected sources, or a percentage?", "Sum,Percentage", false, "Sum", Order = 5 )]
    [TextField( "Secondary Metric Key", "If this is used, do not select a secondary metric source", Order = 6 )]
    [MetricCategoriesField( "Secondary Metric Source", "Only used this if you are creating a Percentage comparison.", Order = 7 )]
    [CustomCheckboxListField( "Respect Page Context", "", "Yes", Order = 8 )]
    //[SlidingDateRangeField( "Date Range", Key = "SlidingDateRange", Order = 9 )]
    //[CustomRadioListField( "Custom Dates", "If not using date range, please select a custom date from here", "This Week Last Year", Order = 9 )]
    //[CustomCheckboxListField( "Compare Against Last Year", "", "Yes", Order = 10 )]
    public partial class MinistryMetrics : RockBlock
    {
        #region Fields

        /// <summary>
        /// Gets or sets the metric block values.
        /// </summary>
        /// <value>
        /// The metric block values.
        /// </value>
        protected string MetricBlockValues
        {
            get
            {
                var viewStateValues = ViewState["MetricBlockValues"] as string;
                return viewStateValues ?? string.Empty;
            }
            set
            {
                ViewState["MetricBlockValues"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the metric should compare to last year.
        /// </summary>
        /// <value>
        /// <c>true</c> if the metric should compare to last year; otherwise, <c>false</c>.
        /// </value>
        //protected string MetricCompareLastYear
        //{
        //    get
        //    {
        //        var viewStateValue = ViewState[string.Format( "MetricCompareLastYear_{0}", BlockId )] as string;
        //        return viewStateValue ?? string.Empty;
        //    }
        //    set
        //    {
        //        ViewState[string.Format( "MetricCompareLastYear_{0}", BlockId )] = value;
        //    }
        //}

        // Let's create null context values so they are available
        protected IEntity CampusContext = new List<IEntity>() as IEntity;

        protected IEntity ScheduleContext = new List<IEntity>() as IEntity;

        protected IEntity GroupContext = new List<IEntity>() as IEntity;

        protected string PrimaryMetricKey = string.Empty;

        protected string SecondaryMetricKey = string.Empty;

        #endregion

        #region Control Methods

        // <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            // check the page context
            var pageContext = GetAttributeValue( "RespectPageContext" ).AsBooleanOrNull();

            // If the blocks respect page context let's set those vars
            if ( pageContext.HasValue )
            {
                // Get Current Campus Context
                CampusContext = RockPage.GetCurrentContext( EntityTypeCache.Read( typeof( Campus ) ) );

                // Get Current Schedule Context
                ScheduleContext = RockPage.GetCurrentContext( EntityTypeCache.Read( typeof( Schedule ) ) );

                // Get Current Group Context
                GroupContext = RockPage.GetCurrentContext( EntityTypeCache.Read( typeof( Group ) ) );
            }

            //var dateRange = SlidingDateRangePicker.CalculateDateRangeFromDelimitedValues( this.GetAttributeValue( "SlidingDateRange" ) ?? string.Empty );

            // Output variables direct to the ascx
            metricBlockNumber.Value = BlockId.ToString();
            metricBlockId.Value = BlockName.Replace( " ", "" ).ToString();
            metricTitle.Value = BlockName;
            metricDisplay.Value = GetAttributeValue( "MetricDisplayType" );
            metricWidth.Value = GetAttributeValue( "NumberofColumns" );

            var metricComparison = GetAttributeValue( "MetricComparison" );

            var metricCustomDates = GetAttributeValue( "CustomDates" );

            PrimaryMetricKey = GetAttributeValue( "PrimaryMetricKey" );

            SecondaryMetricKey = GetAttributeValue( "SecondaryMetricKey" );

            List<int> primaryMetricSource = GetMetricIds( "PrimaryMetricSource" );
            List<int> secondaryMetricSource = GetMetricIds( "SecondaryMetricSource" );
            var churchMetricPeriod = GetAttributeValue( "MetricPeriod" );

            var newMetric = new MetricService( new RockContext() ).GetByIds( primaryMetricSource ).FirstOrDefault();

            // Show the warning if metric source or a metric key is not selected
            if ( !primaryMetricSource.Any() || string.IsNullOrEmpty( PrimaryMetricKey ) )
            {
                churchMetricWarning.Visible = true;
            }

            // This sets the var to do a Week of Year calculation
            var calendar = DateTimeFormatInfo.CurrentInfo.Calendar;

            // Show data if metric source is selected
            if ( newMetric != null || PrimaryMetricKey != "" )
            {
                GetMetricData( pageContext, metricComparison, metricCustomDates, primaryMetricSource, secondaryMetricSource, newMetric, calendar );
            }

            //MetricCompareLastYear = GetAttributeValue( "CompareAgainstLastYear" ).ToString();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Builds the metric query.
        /// </summary>
        /// <param name="metricSource">The metric source.</param>
        /// <param name="dateRange">The date range.</param>
        /// <returns></returns>
        protected decimal BuildMetricQuery( List<int> metricSource, DateRange dateRange )
        {
            var rockContext = new RockContext();
            var metricService = new MetricService( rockContext );
            var metricQueryable = new MetricService( rockContext ).Queryable();

            if ( !string.IsNullOrEmpty( PrimaryMetricKey ) )
            {
                metricQueryable = metricService.Queryable().Where( a => a.Title.EndsWith( PrimaryMetricKey ) );
            }
            else
            {
                metricQueryable = metricService.GetByIds( metricSource );
            }

            var metricValueQueryable = metricQueryable.SelectMany( a => a.MetricValues ).AsQueryable().AsNoTracking();

            // filter by date context
            if ( dateRange != null )
            {
                metricValueQueryable = metricValueQueryable.Where( a => a.MetricValueDateTime >= dateRange.Start && a.MetricValueDateTime <= dateRange.End );
            }

            // filter by campus context
            if ( CampusContext != null )
            {
                metricValueQueryable = metricValueQueryable.Where( a => a.EntityId == CampusContext.Id );
            }

            // filter by schedule context
            if ( ScheduleContext != null )
            {
                var scheduleTime = new ScheduleService( rockContext ).Get( ScheduleContext.Guid ).StartTimeOfDay;
                metricValueQueryable = metricValueQueryable.Where( a => scheduleTime == DbFunctions.CreateTime( a.MetricValueDateTime.Value.Hour, a.MetricValueDateTime.Value.Minute, a.MetricValueDateTime.Value.Second ) );
            }

            // filter by group context
            if ( GroupContext != null )
            {
                metricValueQueryable = metricValueQueryable.Where( a => a.ForeignId == GroupContext.Id );
            }

            var queryResult = metricValueQueryable.Select( a => a.YValue ).ToList();

            return queryResult.Sum() ?? 0;
        }

        /// <summary>
        /// Times the span difference.
        /// </summary>
        /// <param name="dateRange">The date range.</param>
        /// <returns></returns>
        protected int TimeSpanDifference( DateRange dateRange )
        {
            DateTime dateRangeStart = dateRange.Start ?? DateTime.Now;
            DateTime dateRangeEnd = dateRange.End ?? DateTime.Now;

            TimeSpan ts = dateRangeEnd - dateRangeStart;

            return ts.Days + 1;
        }

        /// <summary>
        /// Gets the metric ids.
        /// </summary>
        /// <param name="metricAttribute">The metric attribute.</param>
        /// <returns></returns>
        protected List<int> GetMetricIds( string metricAttribute )
        {
            var metricCategories = MetricCategoriesFieldAttribute.GetValueAsGuidPairs( GetAttributeValue( metricAttribute ) );

            var metricGuids = metricCategories.Select( a => a.MetricGuid ).ToList();
            return new MetricService( new RockContext() ).GetByGuids( metricGuids ).Select( a => a.Id ).ToList();
        }

        /// <summary>
        /// Computes the metric values.
        /// </summary>
        /// <param name="primaryMetricSource">The primary metric source.</param>
        /// <param name="secondaryMetricSource">The secondary metric source.</param>
        /// <param name="dateRange">The date range.</param>
        /// <param name="numberFormat">The number format.</param>
        /// <returns></returns>
        protected decimal ComputeMetricValues( List<int> primaryMetricSource, List<int> secondaryMetricSource, DateRange dateRange, string numberFormat = "sum" )
        {
            if ( numberFormat == "Percentage" )
            {
                decimal primaryMetricValue = BuildMetricQuery( primaryMetricSource, dateRange );
                decimal secondaryMetricValue = BuildMetricQuery( secondaryMetricSource, dateRange );

                if ( primaryMetricValue != 0 && secondaryMetricValue != 0 )
                {
                    decimal percentage = primaryMetricValue / secondaryMetricValue;
                    return percentage * 100;
                }
                else
                {
                    return 0.0M;
                }
            }
            // This is the default function, which is a sum of all the values
            else
            {
                return BuildMetricQuery( primaryMetricSource, dateRange );
            }
        }

        #endregion

        #region Data Calculation

        private void GetMetricData( bool? pageContext, string metricComparison, string metricCustomDates, List<int> primaryMetricSource, List<int> secondaryMetricSource, Metric newMetric, Calendar calendar )
        {
            if ( GetAttributeValue( "MetricDisplayType" ) == "Text" )
            {
                // This is using the date range picker
                if ( dateRange.Start.HasValue && dateRange.End.HasValue )
                {
                    var differenceInDays = TimeSpanDifference( dateRange );

                    var compareMetricValue = new DateRange
                    {
                        Start = dateRange.Start.Value.AddDays( -differenceInDays ),
                        End = dateRange.End.Value.AddDays( -differenceInDays )
                    };

                    PrimaryMetricKey = GetAttributeValue( "MetricKey" );

                    decimal? currentRangeMetricValue = ComputeMetricValues( primaryMetricSource, secondaryMetricSource, dateRange, metricComparison );

                    decimal? previousRangeMetricValue = ComputeMetricValues( primaryMetricSource, secondaryMetricSource, compareMetricValue, metricComparison );

                    if ( currentRangeMetricValue == 0 && metricComparison == "Percentage" )
                    {
                        currentMetricValue.Value = "-";
                    }
                    else
                    {
                        currentMetricValue.Value = string.Format( "{0:n0}", currentRangeMetricValue );

                        if ( metricComparison == "Percentage" )
                        {
                            metricComparisonDisplay.Value = "%";
                        }
                    }

                    // Check to make sure that current and previous have a value to compare
                    if ( currentRangeMetricValue.ToString() != "0.0" && previousRangeMetricValue.ToString() != "0.0" )
                    {
                        if ( currentRangeMetricValue > previousRangeMetricValue )
                        {
                            metricClass.Value = "fa-caret-up brand-success";
                        }
                        else if ( currentRangeMetricValue < previousRangeMetricValue )
                        {
                            metricClass.Value = "fa-caret-down brand-danger";
                        }
                    }

                    //if ( MetricCompareLastYear == "Yes" )
                    //{
                    //    var comparePreviousYearMetricValue = new DateRange
                    //    {
                    //        Start = dateRange.Start.Value.AddYears( -1 ),
                    //        End = dateRange.End.Value.AddYears( -1 )
                    //    };

                    //    decimal? previousYearRangeMetricValue = MetricValueFunction( primaryMetricSource, comparePreviousYearMetricValue, campusContext, groupContext, scheduleContext );

                    //    previousMetricValue.Value = string.Format( "{0:n0}", previousYearRangeMetricValue );
                    //}
                }

                // This Week Last Year
                else if ( metricCustomDates == "This Week Last Year" )
                {
                    //currentMetricValue.Value = string.Format( "{0:n0}", newMetric.MetricValues
                    //.Where( a => calendar.GetWeekOfYear( a.MetricValueDateTime.Value.Date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday ) == calendar.GetWeekOfYear( DateTime.Now.AddYears( -1 ).Date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday ) && a.MetricValueDateTime.Value.Year.ToString() == DateTime.Now.AddYears( -1 ).ToString() )
                    //.Select( a => a.YValue )
                    //.Sum()
                    //);
                }
            }
            else if ( GetAttributeValue( "MetricDisplayType" ) == "Line" && newMetric != null )
            {
                var metricLabelsList = new List<string>();
                var metricCurrentYearValues = new List<string>();
                var metricPreviousYearValues = new List<string>();

                // Create empty lists for the search to be performed next
                var metricCurrentYear = new List<MetricJson>();
                var metricPreviousYear = new List<MetricJson>();

                // Search for data if a source is selected
                if ( dateRange.Start.HasValue && dateRange.End.HasValue )
                {
                    if ( CampusContext != null && pageContext.HasValue )
                    {
                        metricCurrentYear = newMetric.MetricValues
                            .Where( a => a.MetricValueDateTime >= dateRange.Start && a.MetricValueDateTime <= dateRange.End && a.EntityId.ToString() == CampusContext.Id.ToString() )
                            .OrderBy( a => a.MetricValueDateTime )
                            .Select( a => new MetricJson
                            {
                                date = a.MetricValueDateTime.Value.Date,
                                week = calendar.GetWeekOfYear( a.MetricValueDateTime.Value.Date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday ),
                                year = a.MetricValueDateTime.Value.Year,
                                value = string.Format( "{0:0}", a.YValue )
                            } )
                            .ToList();

                        if ( GetAttributeValue( "CompareAgainstLastYear" ) == "Yes" )
                        {
                            metricPreviousYear = newMetric.MetricValues
                                .Where( a => a.MetricValueDateTime >= dateRange.Start.Value.AddYears( -1 ) && a.MetricValueDateTime <= dateRange.End.Value.AddYears( -1 ) && a.EntityId.ToString() == CampusContext.Id.ToString() )
                                .OrderBy( a => a.MetricValueDateTime )
                                .Select( a => new MetricJson
                                {
                                    date = a.MetricValueDateTime.Value.Date,
                                    week = calendar.GetWeekOfYear( a.MetricValueDateTime.Value.Date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday ),
                                    year = a.MetricValueDateTime.Value.Year,
                                    value = string.Format( "{0:0}", a.YValue )
                                } )
                                .ToList();
                        }
                    }
                    else
                    {
                        metricCurrentYear = newMetric.MetricValues
                            .Where( a => a.MetricValueDateTime >= dateRange.Start && a.MetricValueDateTime <= dateRange.End )
                            .OrderBy( a => a.MetricValueDateTime )
                            .Select( a => new MetricJson
                            {
                                date = a.MetricValueDateTime.Value.Date,
                                week = calendar.GetWeekOfYear( a.MetricValueDateTime.Value.Date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday ),
                                year = a.MetricValueDateTime.Value.Year,
                                value = string.Format( "{0:0}", a.YValue )
                            } )
                            .ToList();

                        if ( GetAttributeValue( "CompareAgainstLastYear" ) == "Yes" )
                        {
                            metricPreviousYear = newMetric.MetricValues
                                .Where( a => a.MetricValueDateTime >= dateRange.Start.Value.AddYears( -1 ) && a.MetricValueDateTime <= dateRange.End.Value.AddYears( -1 ) )
                                .OrderBy( a => a.MetricValueDateTime )
                                .Select( a => new MetricJson
                                {
                                    date = a.MetricValueDateTime.Value.Date,
                                    week = calendar.GetWeekOfYear( a.MetricValueDateTime.Value.Date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday ),
                                    year = a.MetricValueDateTime.Value.Year,
                                    value = string.Format( "{0:0}", a.YValue )
                                } )
                                .ToList();
                        }
                    }
                }

                foreach ( var currentMetric in metricCurrentYear )
                {
                    metricLabelsList.Add( new DateTime( currentMetric.date.Year, currentMetric.date.Month, currentMetric.date.Day ).ToString( "MMMM dd" ) );
                    metricCurrentYearValues.Add( currentMetric.value );

                    if ( metricPreviousYear.Count != 0 )
                    {
                        var count = 0;

                        foreach ( var previousMetric in metricPreviousYear )
                        {
                            var previousMetricCount = count++;
                            if ( currentMetric.week == previousMetric.week )
                            {
                                metricPreviousYearValues.Add( previousMetric.value );
                                break;
                            }
                            else if ( count == metricPreviousYear.Count )
                            {
                                metricPreviousYearValues.Add( "0" );
                                break;
                            }
                        }
                    }
                    else
                    {
                        metricPreviousYearValues.Add( "0" );
                    }
                }

                metricLabels.Value = "'" + metricLabelsList.AsDelimited( "," ).Replace( ",", "','" ) + "'";

                metricDataPointsCurrent.Value = "'" + metricCurrentYearValues.AsDelimited( "," ).Replace( ",", "','" ) + "'";

                metricDataPointsPrevious.Value = "'" + metricPreviousYearValues.AsDelimited( "," ).Replace( ",", "','" ) + "'";
            }
            else if ( GetAttributeValue( "MetricDisplayType" ) == "Donut" && newMetric != null )
            {
                var donutMetrics = new MetricService( new RockContext() ).GetByIds( primaryMetricSource ).ToArray();

                // Current Week of Year
                var currentWeekOfYear = calendar.GetWeekOfYear( DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday );

                // Last Week
                var lastWeekOfYear = calendar.GetWeekOfYear( DateTime.Now.AddDays( -7 ), CalendarWeekRule.FirstDay, DayOfWeek.Sunday );

                var blockValues = new List<MetricValue>();

                var i = 0;

                // Get the metric values from the donutMetrics
                foreach ( var metricItem in donutMetrics )
                {
                    var metricItemCount = i++;
                    var metricItemTitle = metricItem.Title;

                    // Create empty lists for the search to be performed next
                    var currentWeekMetric = new decimal?();
                    var previousWeekMetric = new decimal?();

                    // Search DB Based on Current Week of Year
                    if ( dateRange.Start.HasValue && dateRange.End.HasValue )
                    {
                        if ( CampusContext != null && pageContext.HasValue )
                        {
                            currentMetricValue.Value = string.Format( "{0:n0}", newMetric.MetricValues
                                .Where( a => a.MetricValueDateTime >= dateRange.Start && a.MetricValueDateTime <= dateRange.End && a.EntityId.ToString() == CampusContext.Id.ToString() )
                                .Select( a => a.YValue )
                                .FirstOrDefault()
                                );
                        }
                        else
                        {
                            currentWeekMetric = metricItem.MetricValues
                                .Where( a => a.MetricValueDateTime >= dateRange.Start && a.MetricValueDateTime <= dateRange.End )
                                .Select( a => a.YValue )
                                .FirstOrDefault();
                        }
                    }
                    else
                    {
                        currentWeekMetric = metricItem.MetricValues
                            .Where( a => calendar.GetWeekOfYear( a.MetricValueDateTime.Value.AddDays( -7 ).Date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday ) == currentWeekOfYear && a.MetricValueDateTime.Value.Year == DateTime.Now.Year )
                            .Select( a => a.YValue )
                            .FirstOrDefault();

                        previousWeekMetric = metricItem.MetricValues
                            .Where( a => calendar.GetWeekOfYear( a.MetricValueDateTime.Value.AddDays( -7 ).Date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday ) == lastWeekOfYear && a.MetricValueDateTime.Value.Year == DateTime.Now.Year )
                            .Select( a => a.YValue )
                            .FirstOrDefault();
                    }

                    // Assign Colors to Var
                    string metricItemColor = "#6bac43";

                    if ( metricItemCount % 2 != 0 )
                    {
                        metricItemColor = "#1c683e";
                    }
                    else if ( metricItemCount % 3 == 0 )
                    {
                        metricItemColor = "#2a4930";
                    }

                    // Create JSON array of data
                    if ( currentWeekMetric != null )
                    {
                        blockValues.Add( new MetricValue() { value = (int)currentWeekMetric.Value, color = metricItemColor, highlight = metricItemColor, label = metricItemTitle } );
                    }
                    else if ( previousWeekMetric != null )
                    {
                        blockValues.Add( new MetricValue() { value = (int)previousWeekMetric.Value, color = metricItemColor, highlight = metricItemColor, label = metricItemTitle } );
                    }
                    else
                    {
                        blockValues.Add( new MetricValue() { value = 0, color = metricItemColor, highlight = metricItemColor, label = metricItemTitle } );
                    }
                }

                MetricBlockValues = JsonConvert.SerializeObject( blockValues.ToArray() );
            }
        }

        #endregion

        #region Classes

        /// <summary>
        /// Metric information used to bind the selected grid.
        /// </summary>
        [Serializable]
        protected class MetricValue
        {
            public int value { get; set; }

            public string color { get; set; }

            public string highlight { get; set; }

            public string label { get; set; }
        }

        /// <summary>
        /// Metric information as a JSON object
        /// </summary>
        [Serializable]
        protected class MetricJson
        {
            public System.DateTime date { get; set; }

            public string value { get; set; }

            public int week { get; set; }

            public int year { get; set; }
        }

        //public static class MyStaticValues
        //{
        //    public static bool metricCompareLastYear { get; set; }
        //}

        public class MetricValueList
        {
            public string Name { get; set; }

            public decimal Value { get; set; }
        }

        #endregion Classes
    }
}