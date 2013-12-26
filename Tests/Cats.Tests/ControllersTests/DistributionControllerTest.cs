﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Cats.Models.Constant;
using Cats.Models.Security;
using Cats.Services.Hub;
using Cats.Services.Security;
using NUnit.Framework;
using Cats.Areas.Logistics.Controllers;
using Moq;
using Cats.Services.Procurement;
using Cats.Services.EarlyWarning;
using Cats.Services.Logistics;
using Cats.Services.Common;
using Cats.Areas.Logistics.Models;
using Cats.Models;
using Cats.Models.Hubs;

namespace Cats.Tests.ControllersTests
{
   public class DistributionControllerTest
    {
        #region Setup

       private DistributionController _distributionController;
      
        [SetUp]
        public void Init()
        {
            var transportOrderService = new Mock<ITransportOrderService>();
            var workflowStatusService = new Mock<IWorkflowStatusService>();
            var dispatchAllocationService = new Mock<IDispatchAllocationService>();
            var distributionService = new Mock<IDistributionService>();
            var dispatchService = new Mock<IDispatchService>();
            var distributionDetailService = new Mock<IDistributionDetailService>();
            var notificationService = new Mock<INotificationService>();
            var userAccountService = new Mock<IUserAccountService>();
            var transportOrders = new List<TransportOrder>()
                                      {
                                          new TransportOrder()
                                              {
                                                  TransporterID = 1,
                                                  BidDocumentNo = "11",
                                                  TransportOrderID = 1,
                                                  TransportOrderNo = "TRO-1",
                                                  StatusID=1,
                                                  Transporter = new Cats.Models.Transporter() {Name = "T2", TransporterID = 1}
                                              }
                                      };
            var workflowstatuses = new List<WorkflowStatus>()
                                       {
                                           new WorkflowStatus()
                                               {
                                                   StatusID = 1,
                                                   Description = "Draft",
                                                   WorkflowID = 1
                                               }
                                       };
            var dispatches = new List<DispatchViewModel>()
                                 {
                                     new DispatchViewModel()
                                         {
                                             RequisitionNo = "REQ-001",
                                             BidNumber = "001",
                                             TransporterID = 1,
                                             ProgramID = 1,
                                             Month = 1,
                                             Round = 1,
                                             QuantityPerUnit = 1,
                                             UnitID = 1,
                                             QuantityInUnit = 1,
                                             Quantity = 1,
                                             ProjectNumber = "001",
                                             ProjectCodeID = 1,
                                             ShippingInstructionID = 1,
                                             SINumber = "001",
                                             UserProfileID = 1,
                                             PlateNo_Prime = "001",
                                             PlateNo_Trailer = "002",
                                             GRNReceived = false,
                                             Transporter = "T2",
                                             Year = 2013,
                                             FDPID = 1,
                                             CommodityID = 1,
                                             FDP = "1",
                                             HubID = 1,
                                             DispatchID = Guid.NewGuid(),
                                             DistributionID = Guid.NewGuid(),
                                             DispatchDate = DateTime.Today,
                                             CreatedDate = DateTime.Today,
                                             DispatchAllocationID = Guid.NewGuid(),

                                         }
                                 };
            var distributions = new List<Distribution>()
                                    {
                                        new Distribution()
                                            {
                                                DeliveryDate = DateTime.Today,
                                                DeliveryBy ="Ban",
                                                DonorID = 1,
                                                FDPID=1,
                                                HubID=1,
                                                InvoiceNo="002",
                                                TransporterID=1,
                                                ReceivedDate = DateTime.Today,
                                                ReceivingNumber="002",
                                                DispatchID=Guid.NewGuid(),
                                                DistributionID = Guid.NewGuid(),
                                                DocumentReceivedDate = DateTime.Today
                                                
           
                                                          
                                            }
                                    };
            var user = new UserInfo() {UserProfileID = 1, DatePreference = "GC"};
            transportOrderService.Setup(
                t =>
                t.Get(It.IsAny<Expression<Func<TransportOrder, bool>>>(),
                      It.IsAny<Func<IQueryable<TransportOrder>, IOrderedQueryable<TransportOrder>>>(),
                      It.IsAny<string>())).Returns(transportOrders);
            workflowStatusService.Setup(t => t.GetStatus(It.IsAny<WORKFLOW>())).Returns(workflowstatuses);
            dispatchAllocationService.Setup(t => t.GetTransportOrderDispatches(It.IsAny<int>())).Returns(dispatches);
            distributionService.Setup(t => t.FindBy(It.IsAny<Expression<Func<Distribution, bool>>>())).Returns(
                distributions);
            userAccountService.Setup(t => t.GetUserInfo(It.IsAny<string>())).Returns(user);


                                                                                           
            var fakeContext = new Mock<HttpContextBase>();
            var identity = new GenericIdentity("User");
            var principal = new GenericPrincipal(identity,null);
            fakeContext.Setup(t => t.User).Returns(principal);
            
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(t => t.HttpContext).Returns(fakeContext.Object);
            
            
            _distributionController=
               new DistributionController(
                   transportOrderService.Object,
                   workflowStatusService.Object,
                   dispatchAllocationService.Object,
                   distributionService.Object,
                   dispatchService.Object,
                   distributionDetailService.Object,
                   notificationService.Object,
                   userAccountService.Object
               );
            _distributionController.ControllerContext = controllerContext.Object;

        }

        [TearDown]
        public void Dispose()
        {
           _distributionController.Dispose();
        }
        #endregion

        #region Tests
       
     [Test]
       public void CanShowDispatchForTransportOrder()
     {
         var transportOrderId = 1;
         var result =(ViewResult) _distributionController.Dispatches(transportOrderId);
         Assert.IsInstanceOf<TransportOrderDispatchViewModel>(result.Model);
     }
       
        #endregion
    }
}