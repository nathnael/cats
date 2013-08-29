﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cats.Helpers;
using Cats.Models;
using Cats.Models.Constant;
using Cats.Models.ViewModels;
using Cats.Models.ViewModels.HRD;
using Cats.Services.EarlyWarning;
using Cats.Services.Security;
using Cats.ViewModelBinder;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Cats.Areas.EarlyWarning.Controllers
{
    public class HRDController :Controller
    {
        //
        // GET: /EarlyWarning/HRD/
        private IAdminUnitService _adminUnitService;
        private IHRDService _hrdService;
        private IRationService _rationService;
        private IHRDDetailService _hrdDetailService;
        private ICommodityService _commodityService;
        private IRationDetailService _rationDetailService;
        private INeedAssessmentDetailService _needAssessmentDetailService;
        private INeedAssessmentHeaderService _needAssessmentService;
        private IWorkflowStatusService _workflowStatusService;
        private ISeasonService _seasonService;
        private IUserAccountService _userAccountService;

        public HRDController(IAdminUnitService adminUnitService, IHRDService hrdService,
                             IRationService rationservice, IRationDetailService rationDetailService,
                             IHRDDetailService hrdDetailService, ICommodityService commodityService,
                             INeedAssessmentDetailService needAssessmentDetailService, INeedAssessmentHeaderService needAssessmentService,
                             IWorkflowStatusService workflowStatusService, ISeasonService seasonService)
        {
            _adminUnitService = adminUnitService;
            _hrdService = hrdService;
            _hrdDetailService = hrdDetailService;
            _commodityService = commodityService;
            _rationService = rationservice;
            _rationDetailService = rationDetailService;
            _needAssessmentDetailService = needAssessmentDetailService;
            _needAssessmentService = needAssessmentService;
            _workflowStatusService = workflowStatusService;
            _seasonService = seasonService;
        }

        public ActionResult Index()
        {
            var hrd = _hrdService.GetAllHRD();
            //ViewBag.Status = _workflowStatusService.GetStatusName();
            return View(hrd);
        }
        public ActionResult HRDDetail(int id = 0)
        {
            ViewData["Month"] = RequestHelper.GetMonthList();
            var hrd = _hrdService.Get(m => m.HRDID == id, null, "HRDDetails").FirstOrDefault();
            ViewBag.SeasonID = hrd.Season.Name;
            ViewBag.Year = hrd.Year;

            if (hrd != null)
            {
                return View(hrd);
            }
            return RedirectToAction("Index");
        }
        public ActionResult ApprovedHRDs()
        {
            return View();
        }

        public ActionResult CurrentHRDs()
        {
            return View();
        }
        public ActionResult HRD_Read([DataSourceRequest] DataSourceRequest request)
        {

            var hrds = _hrdService.Get(m=>m.Status==1).OrderByDescending(m => m.HRDID);
            var hrdsToDisplay = GetHrds(hrds).ToList();
            return Json(hrdsToDisplay.ToDataSourceResult(request));
        }

        public ActionResult HRDDetail_Read([DataSourceRequest] DataSourceRequest request, int id = 0)
        {


            //var hrdDetail = _hrdService.GetHRDDetailByHRDID(id).OrderBy(m => m.AdminUnit.AdminUnit2.Name).OrderBy(m => m.AdminUnit.AdminUnit2.AdminUnit2.Name);
            var hrd = _hrdService.Get(m => m.HRDID == id, null, "HRDDetails").FirstOrDefault();

            if (hrd != null)
            {
                var detailsToDisplay = GetHRDDetails(hrd).ToList();
                return Json(detailsToDisplay.ToDataSourceResult(request));
            }
            return RedirectToAction("Index");
        }

        public ActionResult ApprovedHRD_Read([DataSourceRequest] DataSourceRequest request)
        {

            var hrds = _hrdService.Get(m => m.Status == 2).OrderByDescending(m => m.HRDID);
            var hrdsToDisplay = GetHrds(hrds).ToList();
            return Json(hrdsToDisplay.ToDataSourceResult(request));
        }
        //get published hrds information
        public ActionResult CurrentHRD_Read([DataSourceRequest] DataSourceRequest request)
        {
            DateTime latestDate = _hrdService.Get(m => m.Status == 3).Max(m => m.PublishedDate);
            var hrds = _hrdService.FindBy(m =>m.Status==3 && m.PublishedDate == latestDate);
                //.OrderBy(m => m.PublishedDate);
            var hrdsToDisplay = GetHrds(hrds).ToList();
            return Json(hrdsToDisplay.ToDataSourceResult(request));
        }

        //gets hrd information
        private IEnumerable<HRDViewModel> GetHrds(IEnumerable<HRD> hrds)
        {
            return (from hrd in hrds
                    let statusID = hrd.Status
                    where statusID != null
                    select new HRDViewModel()
                    {
                        HRDID = hrd.HRDID,
                        Season = hrd.Season.Name,
                        Year = hrd.Year,
                        Ration = hrd.Ration.RefrenceNumber,
                        CreatedDate = hrd.CreatedDate,
                        CreatedBy = hrd.UserProfile.FirstName + " " + hrd.UserProfile.LastName,
                        PublishedDate = hrd.PublishedDate,
                        StatusID = statusID!=0? statusID:1,
                        Status = _workflowStatusService.GetStatusName(WORKFLOW.HRD, statusID.Value)


                    });
        }

        public ActionResult RegionalSummary_Read([DataSourceRequest] DataSourceRequest request, int id = 0)
        {
            var hrd = _hrdService.Get(m => m.HRDID == id, null, "HRDDetails").FirstOrDefault();

            if (hrd != null)
            {
                var detailsToDisplay = GetSummary(hrd).ToList();
                return Json(detailsToDisplay.ToDataSourceResult(request));
            }
            return RedirectToAction("Index");
        }

        public ActionResult RegionalSummary(int id=0)
        {
            
            var hrd = _hrdService.Get(m => m.HRDID == id).FirstOrDefault();
            ViewBag.SeasonID = hrd.Season.Name;
            ViewBag.Year = hrd.Year;

            
                return View(hrd);
            
        }
        private IEnumerable<RegionalSummaryViewModel> GetSummary(HRD hrd)
        {
            var details = hrd.HRDDetails;
            //var hrd = _hrdService.FindById(id);
            //details.First().HRD;
            var cerealCoefficient = hrd.Ration.RationDetails.First(m => m.Commodity.CommodityID == 1).Amount;
            var blendFoodCoefficient = hrd.Ration.RationDetails.First(m => m.Commodity.CommodityID == 2).Amount;
            var pulseCoefficient = hrd.Ration.RationDetails.First(m => m.Commodity.CommodityID == 3).Amount;
            var oilCoefficient = hrd.Ration.RationDetails.First(m => m.Commodity.CommodityID == 4).Amount;

            ViewBag.SeasonID = hrd.Season.Name;
            ViewBag.Year = hrd.Year;

            var groupedTotal = from detail in details
                               group detail by detail.AdminUnit.AdminUnit2.AdminUnit2 into regionalDetail
                               select new
                               {
                                   Region = regionalDetail.Key,
                                   NumberOfBeneficiaries = regionalDetail.Sum(m => m.NumberOfBeneficiaries),
                                   Duration = regionalDetail.FirstOrDefault().DurationOfAssistance
                               };
            return (from total in groupedTotal
                            select new RegionalSummaryViewModel
                                {
                                    RegionName = total.Region.Name,
                                    NumberOfBeneficiaries = total.NumberOfBeneficiaries,
                                    Cereal = cerealCoefficient * total.NumberOfBeneficiaries * total.Duration,
                                    BlededFood = blendFoodCoefficient * total.NumberOfBeneficiaries * total.Duration,
                                    Oil = oilCoefficient * total.NumberOfBeneficiaries * total.Duration,
                                    Pulse = pulseCoefficient * total.NumberOfBeneficiaries * total.Duration
                                });
                
        }

        private IEnumerable<HRDDetailViewModel> GetHRDDetails(HRD hrd)
        {
            var hrdDetails = hrd.HRDDetails;
            var rationDetails = _rationService.FindById(hrd.RationID).RationDetails;
            return (from hrdDetail in hrdDetails
                    select new HRDDetailViewModel()
                    {
                        HRDDetailID = hrdDetail.HRDDetailID,
                        HRDID = hrdDetail.HRDID,
                        WoredaID = hrdDetail.WoredaID,
                        Zone = hrdDetail.AdminUnit.AdminUnit2.Name,
                        Region = hrdDetail.AdminUnit.AdminUnit2.AdminUnit2.Name,
                        Woreda = hrdDetail.AdminUnit.Name,
                        NumberOfBeneficiaries = hrdDetail.NumberOfBeneficiaries,
                        //(int)GetTotalBeneficiaries(hrdDetail.HRDID, hrdDetail.AdminUnit.AdminUnit2.AdminUnit2.AdminUnitID),
                        StartingMonth = hrdDetail.StartingMonth,
                        DurationOfAssistance = hrdDetail.DurationOfAssistance,
                        Cereal = (hrdDetail.DurationOfAssistance) * (hrdDetail.NumberOfBeneficiaries) * (rationDetails.Single(m => m.CommodityID == 1).Amount),
                        Pulse = (hrdDetail.DurationOfAssistance) * (hrdDetail.NumberOfBeneficiaries) * (rationDetails.Single(m => m.CommodityID == 2).Amount),
                        BlendedFood = (hrdDetail.DurationOfAssistance) * (hrdDetail.NumberOfBeneficiaries) * (rationDetails.Single(m => m.CommodityID == 3).Amount),
                        Oil = (hrdDetail.DurationOfAssistance) * (hrdDetail.NumberOfBeneficiaries) * (rationDetails.Single(m => m.CommodityID == 4).Amount)


                    });
        }
        public ActionResult Create()
        {
            var hrd = new HRD();
            // hrd.HRDDetails = new List<HRDDetail>();
            ViewBag.Year = DateTime.Today.Year;
            ViewBag.RationID = new SelectList(_rationService.GetAllRation(), "RationID", "RefrenceNumber", hrd.RationID = 1);
            ViewBag.NeedAssessmentID = new SelectList(_needAssessmentService.GetAllNeedAssessmentHeader().Where(m => m.NeedAssessment.NeedAApproved == true), "NAHeaderId",
                                                      "NeedACreatedDate");

            ViewData["SeasonID"] = new SelectList(_seasonService.GetAllSeason(), "SeasonID", "Name");
            var woredas = _adminUnitService.FindBy(m => m.AdminUnitTypeID == 3);
            var commodities = _commodityService.GetAllCommodity();

            var hrdDetails = (from detail in woredas
                              select new HRDDetail()
                              {
                                  WoredaID = detail.AdminUnitID,
                                  NumberOfBeneficiaries = 0

                              }).ToList();
            hrd.HRDDetails = hrdDetails;


            return View(hrd);
        }

        public JsonResult GetRation()
        {


            var ration = _rationService.Get(t => t.IsDefaultRation, null, "RationDetails").FirstOrDefault();
            var rationViewModel = (from item in ration.RationDetails
                                   select new
                                              {
                                                  _commodityService.FindById(item.CommodityID).Name,
                                                  Value = item.Amount
                                              });
            return Json(rationViewModel, JsonRequestBehavior.AllowGet);
        }
        //update HRD detail information
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HRDDetail_Update([DataSourceRequest] DataSourceRequest request, HRDDetailViewModel hrdDetails)
        {
            if (hrdDetails != null && ModelState.IsValid)
            {
                var detail = _hrdDetailService.FindById(hrdDetails.HRDDetailID);
                if (detail != null)
                {
                    detail.HRDID = hrdDetails.HRDID;
                    detail.DurationOfAssistance = hrdDetails.DurationOfAssistance;
                    detail.NumberOfBeneficiaries = hrdDetails.NumberOfBeneficiaries;
                    detail.StartingMonth = hrdDetails.StartingMonth;
                    detail.WoredaID = hrdDetails.WoredaID;

                    _hrdDetailService.EditHRDDetail(detail);
                }

            }
            return Json(new[] { hrdDetails }.ToDataSourceResult(request, ModelState));
            //return Json(ModelState.ToDataSourceResult());
        }
        private DateTime GetGregorianDate(string ethiopianDate)
        {
            DateTime convertedGregorianDate;
            try
            {
                convertedGregorianDate = DateTime.Parse(ethiopianDate);
            }
            catch (Exception ex)
            {
                var strEth = new getGregorianDate();
                convertedGregorianDate = strEth.ReturnGregorianDate(ethiopianDate);
            }
            return convertedGregorianDate;
        }

        [HttpPost]
        public ActionResult Create(HRD hrd)
        {
            DateTime dateCreated = DateTime.Now;
            DateTime DatePublished = DateTime.Now;

            //dateCreated = GetGregorianDate(create);
            //DatePublished = GetGregorianDate(published);

            hrd.CreatedDate = dateCreated;
            hrd.PublishedDate = DatePublished;
            hrd.Status = 1;

            if (ModelState.IsValid)
            {

                var userid = _needAssessmentService.GetUserProfileId(HttpContext.User.Identity.Name);
                // UserAccountHelper.GetUser(HttpContext.User.Identity.Name).UserAccountId;
                var woredas = _adminUnitService.FindBy(m => m.AdminUnitTypeID == 4);
                //var commodities = _commodityService.GetCommonCommodity();
                //_commodityService.Get(m=>m.CommodityID==1 && m.CommodityID==2 && m.CommodityID==4 && m.CommodityID==8);
                var seasonID = hrd.SeasonID;

                hrd.CreatedBY = userid;
                var seasonId = hrd.SeasonID;
                //var hrdDetails = new List<HRDDetail>();
                //foreach (var adminUnit in woredas)
                //{
                   
                //    var detail = new HRDDetail();
                //    detail.WoredaID = adminUnit.AdminUnitID;
                //    detail.StartingMonth = 1;
                //    detail.NumberOfBeneficiaries = _needAssessmentDetailService.GetNeedAssessmentBeneficiaryNo(hrd.Year, "Meher", adminUnit.AdminUnitID);
                //    detail.DurationOfAssistance = _needAssessmentDetailService.GetNeedAssessmentMonths(hrd.Year, "Meher", adminUnit.AdminUnitID);
                //    hrdDetails.Add(detail);
                //}
                var hrdDetails = (from detail in woredas
                                  select new HRDDetail()
                                  {
                                      WoredaID = detail.AdminUnitID,
                                      StartingMonth = 1,

                                      NumberOfBeneficiaries = _needAssessmentDetailService.GetNeedAssessmentBeneficiaryNo(hrd.Year, (int) seasonID, detail.AdminUnitID),
                                      DurationOfAssistance = _needAssessmentDetailService.GetNeedAssessmentMonths(hrd.Year, (int) seasonID, detail.AdminUnitID)
                                 }).ToList();

                hrd.HRDDetails = hrdDetails;
                _hrdService.AddHRD(hrd);

            }

            return RedirectToAction("Index");
        }
        //HRD/Edit/2
        public ActionResult Edit(int id)
        {
            var hrd = _hrdService.Get(m => m.HRDID == id, null, "HRDDetails").FirstOrDefault();
            ViewBag.Month = new SelectList(_seasonService.GetAllSeason(), "SeasonID", "Name", hrd.SeasonID);
            ViewBag.RationID = new SelectList(_rationService.GetAllRation(), "RationID", "RefrenceNumber", hrd.RationID);
            //ViewBag.NeedAssessmentID = new SelectList(_needAssessmentService.GetAllNeedAssessmentHeader(), "NAHeaderId", "NeedACreatedDate", hrd.NeedAssessmentID);


            return View(hrd);
        }

        [HttpPost]
        public ActionResult Edit(HRD hrd)
        {
            var userid = UserAccountHelper.GetUser(HttpContext.User.Identity.Name).UserAccountId;
            hrd.CreatedBY = userid;
            if (ModelState.IsValid)
            {
                _hrdService.EditHRD(hrd);
                return RedirectToAction("Index");
            }

            return View(hrd);
        }

        public ActionResult Print()
        {
            var allHrd = _hrdService.GetAllHRD();
            var hrdViewModel = GetHrds(allHrd);
            return View();//ViewPdf("HRD report", "Print", hrdViewModel);
        }

        public ActionResult ApproveHRD(int id)
        {
            var hrd = _hrdService.FindById(id);
            hrd.Status = 2;
            _hrdService.EditHRD(hrd);
            return RedirectToAction("Index");
        }
        public ActionResult PublishHRD(int id)
        {
            var hrd = _hrdService.FindById(id);
            hrd.Status = 3;
            hrd.PublishedDate = DateTime.Now;
            _hrdService.EditHRD(hrd);
            return RedirectToAction("ApprovedHRDs");
        }

        public ActionResult Compare()
        {
            var hrds = _hrdService.Get(null,null,"Season");
            var hrds1 =
                (from item in hrds
                 select new {item.HRDID, Name = string.Format("{0}-{1}", item.Season.Name, item.Year)}).ToList();
            ViewBag.firstHrd =new SelectList(hrds1,"HRDID","Name");
            ViewBag.secondHrd = new SelectList(hrds1, "HRDID", "Name");
            ViewBag.regionId = new SelectList(_adminUnitService.GetRegions(), "AdminUnitID", "Name");

            return View();
        }

       
        [HttpPost]
        public ActionResult Compare_HRD([DataSourceRequest] DataSourceRequest request,int? firstHrd, int? secondHrd, int? regionId)
        {
            int hrd1Id = firstHrd ?? 0;
            int hrd2Id = secondHrd ?? 0;
            int regionid = regionId ?? 0;

            var hrdFirst = _hrdService.Get(t => t.HRDID == hrd1Id, null,
                                      "Season,HRDDetails,HRDDetails.AdminUnit,HRDDetails.AdminUnit.AdminUnit2,HRDDetails.AdminUnit.AdminUnit2.AdminUnit2").FirstOrDefault();
            var hrdSecond = _hrdService.Get(t => t.HRDID == hrd2Id).FirstOrDefault();
            var hrdsViewModel = HRDViewModelBinder.BindHRDCompareViewModel(hrdFirst, hrdSecond, regionid).OrderBy(t=>t.Zone);


            var hrds = _hrdService.Get(null, null, "Season");
            var hrds1 =
                (from item in hrds
                 select new { item.HRDID, Name = string.Format("{0}-{1}", item.Season.Name, item.Year) }).ToList();
            ViewBag.firstHrd = new SelectList(hrds1, "HRDID", "Name");
            ViewBag.secondHrd = new SelectList(hrds1, "HRDID", "Name");
            ViewBag.redionId = new SelectList(_adminUnitService.GetRegions(), "AdminUnitID", "Name");
            return Json(hrdsViewModel.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
           
        }
    }
}
