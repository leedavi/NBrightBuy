﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Razor;
using System.Web.Script.Serialization;
using System.Windows.Forms.VisualStyles;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using NBrightCore.common;
using NBrightCore.images;
using NBrightCore.render;
using NBrightDNN;
using Nevoweb.DNN.NBrightBuy.Admin;
using Nevoweb.DNN.NBrightBuy.Components.Interfaces;

namespace Nevoweb.DNN.NBrightBuy.Components.Plugins
{
    public static class PluginFunctions
    {
        #region "Admin Methods"

        public static string TemplateRelPath = "/DesktopModules/NBright/NBrightBuy";

        public static string ProcessCommand(string paramCmd, HttpContext context, string editlang = "")
        {
            var strOut = "PLUGIN - ERROR!! - No Security rights or function command.";
            var ajaxInfo = NBrightBuyUtils.GetAjaxFields(context);
            var userId = ajaxInfo.GetXmlPropertyInt("genxml/hidden/userid");

            switch (paramCmd)
            {
                case "plugins_admin_getlist":
                    if (!NBrightBuyUtils.CheckRights()) break;
                    strOut = PluginAdminList(context);
                    break;
                case "plugins_admin_getdetail":
                    if (!NBrightBuyUtils.CheckRights()) break;
                    strOut = PluginAdminDetail(context);
                    break;
                case "plugins_addpluginsmodels":
                    if (!NBrightBuyUtils.CheckRights()) break;
                    PluginAddInterface(context);
                    strOut = PluginAdminDetail(context);
                    break;                    
            }
            return strOut;
        }


        #endregion


        public static string PluginAdminList(HttpContext context)
        {

            try
            {
                if (NBrightBuyUtils.CheckRights())
                {
                    var ajaxInfo = NBrightBuyUtils.GetAjaxInfo(context);
                    var list =  PluginUtils.GetPluginList();
                    return RenderPluginAdminList(list, ajaxInfo, 0);

                }
            }
            catch (Exception ex)
            {
                Logging.LogException(ex);
                return ex.ToString();
            }
            return "";
        }

        public static String RenderPluginAdminList(List<NBrightInfo> list, NBrightInfo ajaxInfo, int recordCount)
        {

            try
            {
                if (NBrightBuyUtils.CheckRights())
                {
                    if (list == null) return "";
                    var strOut = "";

                    // select a specific entity data type for the product (used by plugins)
                    var themeFolder = ajaxInfo.GetXmlProperty("genxml/hidden/themefolder");
                    if (themeFolder == "") themeFolder = "config";
                    var razortemplate = ajaxInfo.GetXmlProperty("genxml/hidden/razortemplate");

                    var passSettings = new Dictionary<string, string>();
                    foreach (var s in ajaxInfo.ToDictionary())
                    {
                        passSettings.Add(s.Key, s.Value);
                    }
                    foreach (var s in StoreSettings.Current.Settings()) // copy store setting, otherwise we get a byRef assignement
                    {
                        if (passSettings.ContainsKey(s.Key))
                            passSettings[s.Key] = s.Value;
                        else
                            passSettings.Add(s.Key, s.Value);
                    }

                    strOut = NBrightBuyUtils.RazorTemplRenderList(razortemplate, 0, "", list, TemplateRelPath, themeFolder, Utils.GetCurrentCulture(), passSettings);

                    return strOut;

                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public static String PluginAdminDetail(HttpContext context)
        {
            try
            {
                if (NBrightBuyUtils.CheckRights())
                {
                    var ajaxInfo = NBrightBuyUtils.GetAjaxInfo(context);

                    var strOut = "";
                    var selecteditemid = ajaxInfo.GetXmlProperty("genxml/hidden/selecteditemid");
                    if (Utils.IsNumeric(selecteditemid))
                    {
                        var themeFolder = ajaxInfo.GetXmlProperty("genxml/hidden/themefolder");
                        if (themeFolder == "") themeFolder = "config";
                        var razortemplate = ajaxInfo.GetXmlProperty("genxml/hidden/razortemplate");

                        var passSettings =  NBrightBuyUtils.GetPassSettings(ajaxInfo);

                        var objCtrl = new NBrightBuyController();
                        var info = objCtrl.GetData(Convert.ToInt32(selecteditemid));
                        var pluginRecord = new PluginRecord(info);

                        strOut = NBrightBuyUtils.RazorTemplRender(razortemplate, 0, "", pluginRecord, TemplateRelPath, themeFolder, Utils.GetCurrentCulture(), passSettings);
                    }
                    return strOut;
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void PluginAddInterface(HttpContext context)
        {
            try
            {
                if (NBrightBuyUtils.CheckRights())
                {
                    var ajaxInfo = NBrightBuyUtils.GetAjaxInfo(context);
                    var selecteditemid = ajaxInfo.GetXmlProperty("genxml/hidden/selecteditemid");
                    if (Utils.IsNumeric(selecteditemid))
                    {
                        var objCtrl = new NBrightBuyController();
                        var info = objCtrl.GetData(Convert.ToInt32(selecteditemid));
                        var pluginRecord = new PluginRecord(info);
                        pluginRecord.AddInterface();
                    }
                }
            }
            catch (Exception ex)
            {
                // ignore
            }
        }

    }
}
