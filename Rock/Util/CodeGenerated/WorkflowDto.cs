//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
//
// THIS WORK IS LICENSED UNDER A CREATIVE COMMONS ATTRIBUTION-NONCOMMERCIAL-
// SHAREALIKE 3.0 UNPORTED LICENSE:
// http://creativecommons.org/licenses/by-nc-sa/3.0/
//
using System;
using System.Collections.Generic;
using System.Dynamic;

using Rock.Data;

namespace Rock.Util
{
    /// <summary>
    /// Data Transfer Object for Workflow object
    /// </summary>
    public partial class WorkflowDto : IDto
    {

#pragma warning disable 1591
        public int WorkflowTypeId { get; set; }
        public string Status { get; set; }
        public bool IsProcessing { get; set; }
        public DateTime? ActivatedDateTime { get; set; }
        public DateTime? LastProcessedDateTime { get; set; }
        public DateTime? CompletedDateTime { get; set; }
        public ICollection<Activity> Activities { get; set; }
        public ICollection<WorkflowLog> LogEntries { get; set; }
        public int Id { get; set; }
        public Guid Guid { get; set; }
#pragma warning restore 1591

        /// <summary>
        /// Instantiates a new DTO object
        /// </summary>
        public WorkflowDto ()
        {
        }

        /// <summary>
        /// Instantiates a new DTO object from the entity
        /// </summary>
        /// <param name="workflow"></param>
        public WorkflowDto ( Workflow workflow )
        {
            CopyFromModel( workflow );
        }

        /// <summary>
        /// Creates a dictionary object.
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>();
            dictionary.Add( "WorkflowTypeId", this.WorkflowTypeId );
            dictionary.Add( "Status", this.Status );
            dictionary.Add( "IsProcessing", this.IsProcessing );
            dictionary.Add( "ActivatedDateTime", this.ActivatedDateTime );
            dictionary.Add( "LastProcessedDateTime", this.LastProcessedDateTime );
            dictionary.Add( "CompletedDateTime", this.CompletedDateTime );
            dictionary.Add( "Activities", this.Activities );
            dictionary.Add( "LogEntries", this.LogEntries );
            dictionary.Add( "Id", this.Id );
            dictionary.Add( "Guid", this.Guid );
            return dictionary;
        }

        /// <summary>
        /// Creates a dynamic object.
        /// </summary>
        /// <returns></returns>
        public virtual dynamic ToDynamic()
        {
            dynamic expando = new ExpandoObject();
            expando.WorkflowTypeId = this.WorkflowTypeId;
            expando.Status = this.Status;
            expando.IsProcessing = this.IsProcessing;
            expando.ActivatedDateTime = this.ActivatedDateTime;
            expando.LastProcessedDateTime = this.LastProcessedDateTime;
            expando.CompletedDateTime = this.CompletedDateTime;
            expando.Activities = this.Activities;
            expando.LogEntries = this.LogEntries;
            expando.Id = this.Id;
            expando.Guid = this.Guid;
            return expando;
        }

        /// <summary>
        /// Copies the model property values to the DTO properties
        /// </summary>
        /// <param name="model">The model.</param>
        public void CopyFromModel( IEntity model )
        {
            if ( model is Workflow )
            {
                var workflow = (Workflow)model;
                this.WorkflowTypeId = workflow.WorkflowTypeId;
                this.Status = workflow.Status;
                this.IsProcessing = workflow.IsProcessing;
                this.ActivatedDateTime = workflow.ActivatedDateTime;
                this.LastProcessedDateTime = workflow.LastProcessedDateTime;
                this.CompletedDateTime = workflow.CompletedDateTime;
                this.Activities = workflow.Activities;
                this.LogEntries = workflow.LogEntries;
                this.Id = workflow.Id;
                this.Guid = workflow.Guid;
            }
        }

        /// <summary>
        /// Copies the DTO property values to the entity properties
        /// </summary>
        /// <param name="model">The model.</param>
        public void CopyToModel ( IEntity model )
        {
            if ( model is Workflow )
            {
                var workflow = (Workflow)model;
                workflow.WorkflowTypeId = this.WorkflowTypeId;
                workflow.Status = this.Status;
                workflow.IsProcessing = this.IsProcessing;
                workflow.ActivatedDateTime = this.ActivatedDateTime;
                workflow.LastProcessedDateTime = this.LastProcessedDateTime;
                workflow.CompletedDateTime = this.CompletedDateTime;
                workflow.Activities = this.Activities;
                workflow.LogEntries = this.LogEntries;
                workflow.Id = this.Id;
                workflow.Guid = this.Guid;
            }
        }
    }
}
