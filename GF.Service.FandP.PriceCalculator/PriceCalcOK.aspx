<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PriceCalcOK.aspx.cs" Inherits="GF.Service.FandP.PriceCalc.PriceCalc" %>
<%
    //System.IO.StreamWriter sw = System.IO.File.CreateText("L:\\Custom logs\\svc_FandP.PriceCalc\\FandPPriceCalcLog.log");                                                           


    XDocument doc = XDocument.Load(Request.InputStream);
    string xmlRequest = doc.ToString();
    string xmlResponse;
    string error;

    try
    {
        GF.Service.FandP.PriceCalc.PriceCalcService client = new GF.Service.FandP.PriceCalc.PriceCalcService();

        xmlRequest = xmlRequest.Replace("</request>", "<affinity_no>522</affinity_no></request>");

        if (client.CalculatePrice(xmlRequest, out xmlResponse, out error))
        {
            if (xmlResponse == "" || xmlResponse == null)
            {
                Response.Write(error);
            }
            else
            {
                Response.Write(xmlResponse);
            }
        }
        else
        {
            //sw.WriteLine(error);
        }
    }
    catch (Exception exc)
    {
        //sw.WriteLine(exc.ToString());

    }
    finally
    {
        //sw.Close();
    }
%>
