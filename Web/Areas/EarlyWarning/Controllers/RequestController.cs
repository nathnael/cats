﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cats.Areas.EarlyWarning.Models;
using Cats.Models;
using Cats.Services.EarlyWarning;
using Cats.Helpers;


namespace Cats.Areas.EarlyWarning.Controllers
{
    public class RequestController : Controller
    {
        //
        // GET: /EarlyWarning/RegionalRequest/

        private IRegionalRequestService _reliefRequistionService;
        private IFDPService _fdpService;
        //private IRoundService _roundService;
        private IAdminUnitService _adminUnitService;
        private IProgramService _programService;
        private ICommodityService _commodityService;
        private IRegionalRequestDetailService _reliefRequisitionDetailService;
        public RequestController(IRegionalRequestService reliefRequistionService
           , IFDPService fdpService
            , IAdminUnitService adminUnitService,
            IProgramService programService,
            ICommodityService commodityService,
            IRegionalRequestDetailService reliefRequisitionDetailService)
        {
            this._reliefRequistionService = reliefRequistionService;
            this._adminUnitService = adminUnitService;
            this._commodityService = commodityService;
            this._fdpService = fdpService;
            this._programService = programService;
            this._reliefRequisitionDetailService = reliefRequisitionDetailService;
        }
        public ActionResult Index()
        {
            ViewBag.Months = new SelectList(RequestHelper.GetMonthList(),"Id","Name");

            var reliefrequistions = _reliefRequistionService.Get(null, null, "AdminUnit,Program");
            return View(reliefrequistions.ToList());
        }

        [HttpPost]
        public ActionResult Index(int year, int month)
        {
            // TODO: Filter the collection using incoming parameters
            ViewBag.Months = new SelectList(RequestHelper.GetMonthList(), "Id", "Name");

            var reliefrequistions = _reliefRequistionService.Get(r=>r.RequistionDate.Year==year && r.RequistionDate.Month==month, null, "AdminUnit,Program");

            return View(reliefrequistions.ToList());
        }

        [HttpGet]
        public ActionResult New()
        {
            var relifRequisition = new RegionalRequest();

            ViewBag.RegionID = new SelectList(_adminUnitService.FindBy(t => t.AdminUnitTypeID == 2), "AdminUnitID", "Name");
            ViewBag.ProgramID = new SelectList(_programService.GetAllProgram(), "ProgramID", "Name");
            //ViewBag.RoundID = new SelectList(_roundService.GetAllRound(), "RoundID", "RoundNumber");
            ViewBag.CommodityID = new SelectList(_commodityService.GetAllCommodity(), "CommodityID", "Name");
            ViewBag.FDPID = new SelectList(_fdpService.GetAllFDP(), "FDPID", "Name");

            var fdpList = _fdpService.GetAllFDP();
            var releifDetails = (from fdp in fdpList
                                 select new RegionalRequestDetail()
                                 {
                                     Beneficiaries = 0,
                                     Fdpid = fdp.FDPID,

                                 }).ToList();
            relifRequisition.RegionalRequestDetails = releifDetails;
            // PrepareFdpList(fdpList);


            return View(relifRequisition);
        }

        //
        // GET: /ReliefRequisitoin/Details/5

        public ActionResult Details(int id = 0)
        {
            RegionalRequest reliefrequistion = _reliefRequistionService.Get(t => t.RegionalRequestID == id, null, "AdminUnit,Program").FirstOrDefault();
            if (reliefrequistion == null)
            {
                return HttpNotFound();
            }
            return View(reliefrequistion);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var reliefRequistion =
                _reliefRequistionService.Get(t => t.RegionalRequestID == id, null, "RegionalRequestDetails,RegionalRequestDetails.Fdp," +
                                                                                    "RegionalRequestDetails.Fdp.AdminUnit,RegionalRequestDetails.Fdp.AdminUnit.AdminUnit2").
                    FirstOrDefault();
            ViewBag.CurrentRegion = reliefRequistion.AdminUnit.Name;
            ViewBag.CurrentMonth = reliefRequistion.RequistionDate.Month;
            ViewBag.CurrentRound = reliefRequistion.Round;
            ViewBag.CurrentYear = reliefRequistion.RequistionDate.Year;
            
            var reliefRequistionDetail = reliefRequistion.RegionalRequestDetails;
            var input = (from itm in reliefRequistionDetail
                         select new RequestDetailEdit
                           {
                               RegionalRequestDetailId = itm.RegionalRequestDetailID,
                               RegionalRequestId = itm.RegionalRequestID,
                               Fdp = itm.Fdp.Name,
                               Wereda= itm.Fdp.AdminUnit.Name,
                               Zone= itm.Fdp.AdminUnit.AdminUnit2.Name ,

                               Beneficiaries = itm.Beneficiaries,
                               Input = new RequestDetailEdit.RequestDetailEditInput()
                               {
                                   Number = itm.RegionalRequestDetailID,
                                   Grain = itm.Grain,
                                   Pulse = itm.Pulse,
                                   Oil = itm.Oil,
                                   CSB = itm.CSB,
                                   Beneficiaries = itm.Beneficiaries
                               }
                           });            
            return View(input);
        }
        [HttpPost]
        public ActionResult Edit(List<RequestDetailEdit.RequestDetailEditInput> input)
        {
            var requId = 0;
            foreach (var reliefRequisitionDetailEditInput in input)
            {
              
                var tempReliefRequistionDetail =
                    _reliefRequisitionDetailService.FindById(reliefRequisitionDetailEditInput.Number);
                requId = tempReliefRequistionDetail.RegionalRequestID;
                tempReliefRequistionDetail.Beneficiaries = reliefRequisitionDetailEditInput.Beneficiaries;
                tempReliefRequistionDetail.CSB = reliefRequisitionDetailEditInput.CSB;
                tempReliefRequistionDetail.Oil = reliefRequisitionDetailEditInput.Oil;
                tempReliefRequistionDetail.Grain = reliefRequisitionDetailEditInput.Grain;
                tempReliefRequistionDetail.Pulse = reliefRequisitionDetailEditInput.Pulse;

            }
            _reliefRequisitionDetailService.Save();

            return RedirectToAction("Edit","Request",new {id=requId});
        }


        [HttpPost]
        public ActionResult New(RegionalRequest reliefRequistion)
        {
            
            
            if (ModelState.IsValid)
            {
                //TODO:Filter with selected region
                var fdpList = _fdpService.FindBy(t=>t.AdminUnit.AdminUnit2.ParentID==reliefRequistion.RegionID);
                var releifDetails = (from fdp in fdpList
                                     select new RegionalRequestDetail()
                                     {
                                         Beneficiaries = 0,
                                         Fdpid = fdp.FDPID,
                                         Grain=0,
                                         Pulse=0,
                                         Oil=0,
                                         CSB=0

                                     }).ToList();
                reliefRequistion.RegionalRequestDetails = releifDetails;
                _reliefRequistionService.AddReliefRequistion(reliefRequistion);
                return RedirectToAction("Edit", "Request", new { id = reliefRequistion.RegionalRequestID });
            }
            return View(new RegionalRequest());
        }
    }
}
