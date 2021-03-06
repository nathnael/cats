﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cats.Models;
using Cats.Services.Administration;
using Cats.Web.Adminstration.Models.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Cats.Web.Adminstration.Controllers
{
     [Authorize]
    public class UserWarehouseController : Controller
    {
        private readonly IUserHubService _userHubService;
        private readonly IHubService _hubService;
        private readonly IUserProfileService _userProfileService;

        public UserWarehouseController(IUserHubService userHubService, IHubService hubService, IUserProfileService userProfileService)
        {
            this._userHubService = userHubService;
            this._hubService = hubService;
            this._userProfileService = userProfileService;
        }

        //
        // GET: /UserWarehouse/

        public ViewResult Index()
        {
            ViewData["UserProfile"] = _userProfileService.GetAllUserProfile();
            ViewData["Hub"] = _hubService.GetAllHub();
            return View();
        }

        
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
           
            return Json(GetUserHub().ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserHubCreate([DataSourceRequest] DataSourceRequest request, HubUserViewModel userhub)
        {
            if (userhub != null && ModelState.IsValid)
            {
                var result = BindUserOwner(userhub);
                //int userProfileId = result.UserProfileID;
                //int wareHouseId= result.UserHubID;

                _userHubService.AddUserHub(result);
            }

            return Json(new[] { userhub }.ToDataSourceResult(request, ModelState));
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HubOwnerUpdate([DataSourceRequest] DataSourceRequest request, HubUserViewModel userhub)
        {
            if (userhub != null && ModelState.IsValid)
            {
                UserHub result = _userHubService.FindById(userhub.UserHubID);
                if (result != null)
                {
                    result.UserProfileID = userhub.UserProfileID;
                    result.HubID = userhub.HubID;
                    _userHubService.EditUserHub(result);
                }
            }
            return Json(new[] { userhub }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HubOwnerDestroy([DataSourceRequest] DataSourceRequest request, HubUserViewModel userhub)
        {
            if (userhub != null && ModelState.IsValid)
            {
                var result = _userHubService.FindById(userhub.UserHubID);
                if (result != null)
                {
                    _userHubService.DeleteUserHub(result);
                }
            }
            return Json(ModelState.ToDataSourceResult());
        }





        private IEnumerable<HubUserViewModel> GetUserHub()
        {
            var result = _userHubService.GetAllUserHub();
            var viewModelList = new List<HubUserViewModel>();
            foreach (var hubOwner in result)
            {
                var ownerViewModel = new HubUserViewModel();

                ownerViewModel.HubID = hubOwner.HubID;
                ownerViewModel.UserHubID = hubOwner.UserHubID;
                ownerViewModel.UserProfileID = hubOwner.UserProfileID;
                                         
                                        
                viewModelList.Add(ownerViewModel);
            }

            return viewModelList;
        }

        private UserHub BindUserOwner(HubUserViewModel hubOwnerViewModel)
        {
            if (hubOwnerViewModel == null) return null;
            var hubOwner = new UserHub()
            {
               HubID = hubOwnerViewModel.HubID,
               UserProfileID = hubOwnerViewModel.UserProfileID,
               

            };
            return hubOwner;
        }


        //
        // GET: /UserWarehouse/Create

        // POST: /UserWarehouse/Create

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request ,HubUserViewModel userwarehouse)
        {
            if (ModelState.IsValid && userwarehouse!=null)
            {
                _userHubService.AddUserHub(BindUserOwner(userwarehouse));

               
            
               return Json(new [] { userwarehouse }.ToDataSourceResult(request, ModelState));
            }

            ViewBag.UserProfileID = new SelectList(_userProfileService.GetAllUserProfile(), "UserProfileID", "UserName", userwarehouse.UserProfileID);
            ViewBag.WarehouseID = new SelectList(_hubService.GetAllHub(), "HubID", "Name", userwarehouse.HubID);
            return RedirectToAction("index");
        }

        //
        // GET: /UserWarehouse/Edit/5


        //
        // POST: /UserWarehouse/Edit/5

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit([DataSourceRequest] DataSourceRequest request,HubUserViewModel userwarehouse)
        {
            if (ModelState.IsValid)
            {
                _userHubService.EditUserHub(BindUserOwner(userwarehouse));
               
                return Json(new { success = true });
            }
            ViewBag.UserProfileID = new SelectList(_userProfileService.GetAllUserProfile(), "UserProfileID", "UserName", userwarehouse.UserProfileID);
            ViewBag.WarehouseID = new SelectList(_hubService.GetAllHub(), "HubID", "Name", userwarehouse.HubID);
            return RedirectToAction("index");
        }

        
        //
        // POST: /UserWarehouse/Delete/5

       
        public ActionResult Delete(int id)
        {
            UserHub userwarehouse = _userHubService.FindBy(u => u.UserHubID == id).Single();
            _userHubService.DeleteUserHub(userwarehouse);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _userHubService.Dispose();
            base.Dispose(disposing);
        }
    }
}