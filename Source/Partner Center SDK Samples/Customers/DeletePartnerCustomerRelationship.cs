﻿
// -----------------------------------------------------------------------
// <copyright file="DeleteCustomerFromTipAccount.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Samples.Customers
{
    using Microsoft.Store.PartnerCenter.Models;
    using Microsoft.Store.PartnerCenter.Models.Customers;
    using Microsoft.Store.PartnerCenter.Models.Subscriptions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Deletes a partner customer Relationship production account.
    /// </summary>
    class DeletePartnerCustomerRelationship : BasePartnerScenario
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeletePartnerCustomerRelationship"/> class.
        /// </summary>
        /// <param name="context">The scenario context.</param>
        public DeletePartnerCustomerRelationship(IScenarioContext context) : base("Delete Partner Relationship", context)
        {

        }
        /// <summary>
        /// Executes the scenario.
        /// </summary>
        protected override void RunScenario()
        {
            var partnerOperations = this.Context.UserPartnerOperations;

            // prompt the user the enter the customer ID
            var customerIdToDeleteRelationshipOf = this.Context.ConsoleHelper.ReadNonEmptyString("Please enter the ID of the customer you want to delete the relationship with", "The customer ID can't be empty");

            // Verify that there are no active subscriptions
            ResourceCollection<Subscription> customerSubscriptions = partnerOperations.Customers.ById(customerIdToDeleteRelationshipOf).Subscriptions.Get();
            IList<Subscription> subscriptions = new List<Subscription>(customerSubscriptions.Items);
            foreach (Subscription customerSubscription in subscriptions)
            {
                if (customerSubscription.Status == SubscriptionStatus.Active)
                {
                    this.Context.ConsoleHelper.Warning(String.Format("Subscription with ID :{0}  OfferName: {1} cannot be in active state, ", customerSubscription.Id, customerSubscription.OfferName));
                    this.Context.ConsoleHelper.Warning("Please Suspend all the Subscriptions and try again. Aborting the delete customer relationship operation");
                    this.Context.ConsoleHelper.StopProgress();
                    return;
                }
            }

            // Delete the customer relationship to partner
            this.Context.ConsoleHelper.StartProgress("Deleting customer Partner Relationship");
            Customer customer = new Customer();
            customer.RelationshipToPartner = CustomerPartnerRelationship.None;
            customer = partnerOperations.Customers.ById(customerIdToDeleteRelationshipOf).Patch(customer);
            if (customer.RelationshipToPartner == CustomerPartnerRelationship.None)
            {
                this.Context.ConsoleHelper.Success("Customer Partner Relationship successfully deleted");
            }
            this.Context.ConsoleHelper.StopProgress();
        }
    }
}
