using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using GF.Components.ErrorHandling;
using GF.Service.FandP.PriceCalc.DOM;
using GF.Service.FandP.PriceCalc.Helpers;
using GF.Service.FandP.PriceCalculator.TIAPriceCalcClient;

namespace GF.Service.FandP.PriceCalc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PriceCalcService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PriceCalcService.svc or PriceCalcService.svc.cs at the Solution Explorer and start debugging.
    public class PriceCalcService : IPriceCalcService
    {
        gfcalcResponse TIARes;
        request req = new request();
        response res = new response();
        int ULSumMax = Int32.Parse(ConfigurationManager.AppSettings["ULSumMax"]);
        
        TIAPriceCalcServiceClient client = new TIAPriceCalcServiceClient();
        Exception exc;                    

        #region Public Methods
        public bool CalculatePrice(string xmlRequest, out string xmlResponse, out string error)
        {
            bool methodResult;
            res.produkt = new responseProdukt();

            xmlResponse = null;
            error = null;

            InfosLogger.Info(xmlRequest, "String");

            if (!(methodResult = PriceCalcConverter.Deserialize(xmlRequest, out req, out exc)))
            {
                //It's invalid XML, we can't use it to generate object.
                error = exc.ToString();
                ErrorsLogger.Error(new ErrorInfo(error));
                return false;
            }
            else
            {
                
                //XML is valid and object is generated
                if (req != null)
                {                    
                    //Just to be sure that object request is not empty
                    if (req.insurancerequest.bil != null)
                    {
                        //If bil object populated calculate price for bil product
                        methodResult = CalculatePriceBI(xmlRequest, out TIARes, out error);
                    }
                    else if (req.insurancerequest.fritidshus != null)
                    {
                        //If fritidshus object populated calculate price for sommerhus product
                        methodResult = CalculatePriceSO(xmlRequest, out TIARes, out error);
                    }
                    else if (req.insurancerequest.hus != null)
                    {
                        //If hus object populated calculate price for hus product
                        methodResult = CalculatePriceHU(xmlRequest, out TIARes, out error);
                    }
                    else if (req.insurancerequest.indbo != null)
                    {
                        //If indbo object populated calculate price for basis product
                        //methodResult = CalculatePriceBA(xmlRequest, out TIARes, out error);
                        methodResult = CalculatePriceIN(xmlRequest, out TIARes, out error);
                    }
                    else if (req.insurancerequest.ulykke != null)
                    {
                        //If ulykke object populated calculate price for ulykke product
                        methodResult = CalculatePriceUL(xmlRequest, out TIARes, out error);
                    }
                    //Remqrk!!!
                    //Only acceptable is that is one object is populated, if more than one populated first one will be handled rest of populated objects are ignored.
                }
            }

            //if (methodResult)
            //{
                //If we get answer we have to convert it to text XML
                if (!(methodResult = PriceCalcConverter.Serialize(res, out xmlResponse, out exc)))
                {
                    if (exc != null)
                    {
                        error = exc.ToString();
                    }
                    ErrorsLogger.Error(new ErrorInfo(error));
                    return false;
                }
                else
                {
                    InfosLogger.Info(xmlResponse, "String");
                    return true;
                }
            //}
            //else
            //{
            //    return methodResult;
            //}
        }
        #endregion Public Methods

        #region Private Methods

        private bool CalculatePriceBA(string xmlRequest, out gfcalcResponse TIARes, out string error)
        {
            //Calculate price for indbo
            BA_request baTIAReq = new BA_request();
            TIARes = null;

            if (PriceCalcConverter.ConvertBasisRequest(req, out baTIAReq, out error))
            {
                //F&P request successfully converted to GF request
                if (client.CalculatePriceBA(baTIAReq, out TIARes, out error))
                {
                    //We get positiv answer from TIA Price Calculator
                    res.indbo = new responseIndbo();
                    res.ingenprisgrund = new responseIngenprisgrund();
                    switch(req.affinity_no)
                    {
                        case 522: res.produkt.produktnavn = "indbo_ok"; break;
                        default: res.produkt.produktnavn = "indbo_gf"; break;
                    }
                    if (TIARes.product[0].product_price != null && TIARes.product[0].error_message == "" && req.insurancerequest.indbo.antal_skader_treaar < 3) //Checking answer content + amount of damages
                    {
                        res.statuscode = 0;
                        res.statusdetail = "";
                        res.statusmessage = "";
                        res.ingenprisgrund.tilvalg1 = false;
                        res.ingenprisgrund.tilvalg2 = false;
                        res.ingenprisgrund.tilvalg3 = false;
                        res.ingenprisgrund.tilvalg4 = false;
                        res.ingenprisgrund.tilvalg5 = false;
                        res.ingenprisgrund.tilvalg6 = false;
                        res.ingenprisgrund.andet = false;

                        //res.produkt.produktpris_forudsaetninger = getProductAssumptions(res.produkt.produktnavn);
                        res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.&lt;br&gt;&lt;br&gt;Forudsætning for prisen på forsikringen er;&lt;br&gt;&lt;ul&gt;&lt;li&gt;at ejendommen ikke anvendes til brandfarlig bedrift. &lt;/li&gt;&lt;li&gt;at ejendommen ikke er en landbrugsejendom. &lt;/li&gt;&lt;li&gt;at ejendommen ikke er tækket med strå. &lt;/li&gt;&lt;/ul&gt;";
                        res.produkt.produktsamlerabat_forudsaetninger = "Hvis du samler mindst tre af følgende forsikringer, får du 10 % samlerabat på dem: indbo-, hus-, ulykkes-, sommerhus-, hunde-, knallert- og tingsforsikring. Der gives ikke rabat på studie/uddannelse-, veteran-, MC- og campingvognsforsikring og på forsikringer, der tilbydes i samarbejde med GF's samarbejdspartnere.";
                        res.schemaVersion = "1.14M";

                        res.produkt.produktpris = (double)TIARes.product[0].product_price.price_yearly;
                        res.produkt.produktsamlerabat = (double)TIARes.product[0].product_price.discounted_price_yearly;
                        res.produkt.tilbagebetaling_af_praemie = false;
                        res.indbo.sum = (double)TIARes.product[0].product_line[0].@object[0].risks.risk[0].risk_sum;
                        res.indbo.selvrisiko = (double)TIARes.product[0].product_line[0].@object[0].risks.risk[0].risk_excess;
                        for (int risk = 0; risk < TIARes.product[0].product_line[0].@object[0].risks.risk.Length; risk++)
                        {
                            switch (TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_name)
                            {
                                case "glas_og_kumme": res.indbo.glaskumme = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "pludselig_skade": res.indbo.pludseligskade = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "elektronikforsikring": res.indbo.elektronik = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "rejse_-_eu": res.indbo.rejseeuropa = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "rejse_-_verden": res.indbo.rejseverden = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "rejse_-_afbestiling": res.indbo.afbestillingsforsikring = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                default: break;
                            }
                        }
                    }
                    else
                    {
                        res.statuscode = 1;
                        res.statusdetail = TIARes.product[0].error_message;
                        res.statusmessage = TIARes.product[0].error_message;
                    }

                }
                else
                {
                    error = exc.ToString();
                    ErrorsLogger.Error(new ErrorInfo(error));
                    return false;
                }             
            }
            else
            {
                //Converterings process failed
                res.indbo = new responseIndbo();
                res.ingenprisgrund = new responseIngenprisgrund();
                res.statuscode = 1;
                res.statusdetail = error;
                res.statusmessage = error;
                return false;
            }
            return true;
        }

        private bool CalculatePriceBI(string xmlRequest, out gfcalcResponse TIARes, out string error)
        {
            //Calculate price for bil
            bool addGFHelp;
            BI_request biTIAReq = new BI_request();
            GF_request gfTIAReq = new GF_request();
            TIARes = null;
            if (PriceCalcConverter.ConvertBilRequest(req, out biTIAReq, out error, out addGFHelp))
            {                
                //F&P request successfully converted to GF request
                if (client.CalculatePriceBI(biTIAReq, out TIARes, out error))
                {
                    //We get positiv answer from TIA Price Calculator
                    res.bil = new responseBil();
                    res.ingenprisgrund = new responseIngenprisgrund();
                    /*if (req.insurancerequest.bil.friskade || req.insurancerequest.bil.foererdaekning || req.insurancerequest.bil.udvidetglas)
                    {
                        res.produkt.produktnavn = "bilplus_gf";
                        //res.produkt.produktpris_forudsaetninger = getProductAssumptions(res.produkt.produktnavn);
                        res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvi-dere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.&lt;br /&gt;&lt;br /&gt;Forudsætning for prisen på forsikringen er;&lt;br /&gt;&lt;ul&gt;&lt;li&gt;at du og andre brugere af bilen har lovbefalet kørekort.&lt;/li&gt;&lt;li&gt;at bilen ikke anvendes erhvervsmæssigt.&lt;/li&gt;&lt;li&gt;at der installeres satellit tracker, hvis bilen har en højere værdi end 637.507 kr. (2015).&lt;/li&gt;&lt;li&gt;at bilen ikke har over 260 hk.&lt;/li&gt;&lt;li&gt;at bilens handelsværdi ikke overstiger 743.758 kr. (2015).&lt;/li&gt;&lt;/ul&gt;&lt;br /&gt;Vær opmærksom på, at når du beregner din pris uden samlerabat, går vi ud fra, at du har mindst én af følgende forsikringer hos os:&lt;br /&gt;&lt;br /&gt;Indbo, hus, ulykke, sommerhus, hundeansvar eller ting. &lt;br /&gt;";
                        res.produkt.produktsamlerabat_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.&lt;br /&gt;&lt;br /&gt;Forudsætning for prisen på forsikringen er;&lt;br /&gt;&lt;ul&gt;&lt;li&gt;at du og andre brugere af bilen har lovbefalet kørekort.&lt;/li&gt;&lt;li&gt;at bilen ikke anvendes erhvervsmæssigt.&lt;/li&gt;&lt;li&gt;at der installeres satellit tracker, hvis bilen har en højere værdi end 637.507 kr. (2015).&lt;/li&gt;&lt;li&gt;at bilen ikke har over 260 hk.&lt;/li&gt;&lt;li&gt;at bilens handelsværdi ikke overstiger 743.758 kr. (2015).&lt;/li&gt;&lt;/ul&gt;Vær opmærksom på, at når du beregner din pris med samlerabat, går vi ud fra, at du har mindst én kombination af to forsikringer hos os:&lt;br /&gt;&lt;br /&gt;- Indbo og ulykke.&lt;br /&gt;- Indbo og hus.&lt;br /&gt;- Indbo og sommerhus. &lt;br /&gt;&lt;br /&gt;";                                    
                    }
                    else
                    {
                        res.produkt.produktnavn = "bil_gf";
                        //res.produkt.produktpris_forudsaetninger = getProductAssumptions(res.produkt.produktnavn);
                        res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvi-dere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.&lt;br /&gt;&lt;br /&gt;Forudsætning for prisen på forsikringen er:&lt;br /&gt;&lt;ul&gt;&lt;li&gt;at du og andre brugere af bilen har lovbefalet kørekort. &lt;/li&gt;&lt;li&gt;at bilen ikke anvendes erhvervsmæssigt.&lt;/li&gt;&lt;li&gt;at der installeres satellit tracker, hvis bilen har en højere værdi end 637.507 kr. (2015).&lt;/li&gt;&lt;li&gt;at bilen ikke har over 260 hk.&lt;/li&gt;&lt;li&gt;at bilens handelsværdi ikke overstiger 743.758 kr. (2015).&lt;/li&gt;&lt;/ul&gt;Vær opmærksom på, at når du beregner din pris uden samlerabat, går vi ud fra, at du har mindst én af følgende forsikringer hos os:&lt;br /&gt;&lt;br /&gt;Indbo, hus, ulykke, sommerhus, hundeansvar eller ting.&lt;br /&gt;&lt;br /&gt;&lt;b&gt;Valg af værksted&lt;/b&gt;&lt;br /&gt;hvis din bil bliver skadet skal du benytte et GF Fordelsværksted,. Hvis du vælger ikke at gøre det, har du en ekstra selvrisiko på 1.113 kr. (2015).&lt;br /&gt;&lt;br /&gt;&lt;b&gt;Ekstra selvrisiko&lt;/b&gt;&lt;br /&gt;Får du en skade, som udløser selvrisiko, og som medfører udgift for GF Forsikring, gælder der for alle efterfølgende skader, som udløser selvrisiko og medfører udgift for GF Forsik-ring, en ekstra selvrisiko på 4.048 kr. (2015). Den ekstra selvrisiko er gældende i resten af det år skaden er sket og i det næste forsikringsår. (Forsikringsåret løber fra 1. januar til 31. december).&lt;br/&gt;";
                        res.produkt.produktsamlerabat_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.&lt;br /&gt;&lt;br /&gt;Forudsætning for prisen på forsikringen er:&lt;br /&gt;&lt;br /&gt;&lt;ul&gt;&lt;li&gt;at du og andre brugere af bilen har lovbefalet kørekort. &lt;/li&gt;&lt;li&gt;at bilen ikke anvendes erhvervsmæssigt. &lt;/li&gt;&lt;li&gt;at der installeres satellit tracker, hvis bilen har en højere værdi end 637.507 kr. (2015).&lt;/li&gt;&lt;li&gt;at bilen ikke har over 260 hk. &lt;/li&gt;&lt;li&gt;at bilens handelsværdi ikke overstiger 743.758 kr. (2015).&lt;/li&gt;&lt;/ul&gt;&lt;br /&gt;Vær opmærksom på, at når du beregner din pris med samlerabat, går vi ud fra, at du har mindst én kombination af to forsikringer hos os:&lt;br /&gt;&lt;br /&gt;- Indbo og ulykke.&lt;br /&gt;- Indbo og hus.&lt;br /&gt;- Indbo og sommerhus.&lt;br /&gt;&lt;br /&gt;&lt;b&gt;Valg af værksted&lt;/b&gt;&lt;br /&gt;Hvis din bil bliver skadet skal du benytte et GF Fordelsværksted,. Hvis du vælger ikke at gøre det, har du en ekstra selvrisiko på 1.113 kr. (2015). &lt;br /&gt;&lt;br /&gt;&lt;b&gt;Ekstra selvrisiko&lt;/b&gt;&lt;br /&gt;&lt;br /&gt;Får du en skade, som udløser selvrisiko, og som medfører udgift for GF Forsikring, gælder der for alle efterfølgende skader, som udløser selvrisiko og medfører udgift for GF Forsikring, en ekstra selvrisiko på 4.048 kr. (2015). Den ekstra selvrisiko er gældende i resten af det år skaden er sket og i det næste forsikringsår. (Forsikringsåret løber fra 1. januar til 31. december).";
                    }*/
                    switch (req.affinity_no)
                    {
                        case 522: res.produkt.produktnavn = "bil_ok"; break;
                        default: res.produkt.produktnavn = "bil_gf"; break;
                    }
                    //res.produkt.produktpris_forudsaetninger = getProductAssumptions(res.produkt.produktnavn);
                    //res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvi-dere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.&lt;br /&gt;&lt;br /&gt;Forudsætning for prisen på forsikringen er:&lt;br /&gt;&lt;ul&gt;&lt;li&gt;at du og andre brugere af bilen har lovbefalet kørekort. &lt;/li&gt;&lt;li&gt;at bilen ikke anvendes erhvervsmæssigt.&lt;/li&gt;&lt;li&gt;at der installeres satellit tracker, hvis bilen har en højere værdi end 637.507 kr. (2015).&lt;/li&gt;&lt;li&gt;at bilen ikke har over 260 hk.&lt;/li&gt;&lt;li&gt;at bilens handelsværdi ikke overstiger 743.758 kr. (2015).&lt;/li&gt;&lt;/ul&gt;Vær opmærksom på, at når du beregner din pris uden samlerabat, går vi ud fra, at du har mindst én af følgende forsikringer hos os:&lt;br /&gt;&lt;br /&gt;Indbo, hus, ulykke, sommerhus, hundeansvar eller ting.&lt;br /&gt;&lt;br /&gt;&lt;b&gt;Valg af værksted&lt;/b&gt;&lt;br /&gt;hvis din bil bliver skadet skal du benytte et GF Fordelsværksted,. Hvis du vælger ikke at gøre det, har du en ekstra selvrisiko på 1.113 kr. (2015).&lt;br /&gt;&lt;br /&gt;&lt;b&gt;Ekstra selvrisiko&lt;/b&gt;&lt;br /&gt;Får du en skade, som udløser selvrisiko, og som medfører udgift for GF Forsikring, gælder der for alle efterfølgende skader, som udløser selvrisiko og medfører udgift for GF Forsik-ring, en ekstra selvrisiko på 4.048 kr. (2015). Den ekstra selvrisiko er gældende i resten af det år skaden er sket og i det næste forsikringsår. (Forsikringsåret løber fra 1. januar til 31. december).&lt;br/&gt;";
                    //res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.&lt;br /&gt; &lt;br /&gt; Forudsætning for prisen på forsikringen er: &lt;ul&gt; &lt;li&gt;du og andre brugere af bilen har lovbefalet kørekort.&lt;/li&gt; &lt;li&gt;at bilen ikke anvendes erhvervsmæssigt.&lt;/li&gt; &lt;li&gt;at der installeres satellit tracker, hvis bilen har en højere værdi end 646.125 kr. (2016).&lt;/li&gt; &lt;li&gt;at bilen ikke har over 260 hk.&lt;/li&gt; &lt;li&gt;at bilens handelsværdi ikke overstiger 753.813 kr. (2016).&lt;/li&gt; &lt;/ul&gt; Vær opmærksom på, at når du beregner din pris, går vi ud fra, at du har mindst én af følgende forsikringer hos os: &lt;br /&gt;&lt;br /&gt;Indbo-, hus-, ulykke-, sommerhus-, hunde- eller tingsforsikring.&lt;br /&gt; &lt;br /&gt; &lt;strong&gt;Valg af værksted&lt;/strong&gt; &lt;br /&gt; Hvis din bil bliver skadet skal du benytte et af de værksteder, som GF samarbejder med. Hvis du vælger ikke at gøre det, har du en ekstra selvrisiko på 1.128 kr. (2016).&lt;br /&gt; &lt;br /&gt; &lt;strong&gt;Ekstra selvrisiko&lt;/strong&gt; &lt;br /&gt; Får du en skade, som udløser selvrisiko, og som medfører udgift for GF Forsikring, gælder der for alle efterfølgende skader, som udløser selvrisiko og medfører udgift for GF Forsikring, en ekstra selvrisiko på 4.103 kr. (2016). Den ekstra selvrisiko på 4.103 kr. (2016) gælder ikke for skader omfattet af en eventuelt tilkøbt friskadedækning. Den ekstra selvrisiko er gældende i resten af det år skaden er sket og i det næste forsikringsår. (Forsikringsåret løber fra 1. januar til 31. december).";
                    //res.produkt.produktsamlerabat_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.&lt;br /&gt;&lt;br /&gt;Forudsætning for prisen på forsikringen er:&lt;br /&gt;&lt;br /&gt;&lt;ul&gt;&lt;li&gt;at du og andre brugere af bilen har lovbefalet kørekort. &lt;/li&gt;&lt;li&gt;at bilen ikke anvendes erhvervsmæssigt. &lt;/li&gt;&lt;li&gt;at der installeres satellit tracker, hvis bilen har en højere værdi end 637.507 kr. (2015).&lt;/li&gt;&lt;li&gt;at bilen ikke har over 260 hk. &lt;/li&gt;&lt;li&gt;at bilens handelsværdi ikke overstiger 743.758 kr. (2015).&lt;/li&gt;&lt;/ul&gt;&lt;br /&gt;Vær opmærksom på, at når du beregner din pris med samlerabat, går vi ud fra, at du har mindst én kombination af to forsikringer hos os:&lt;br /&gt;&lt;br /&gt;- Indbo og ulykke.&lt;br /&gt;- Indbo og hus.&lt;br /&gt;- Indbo og sommerhus.&lt;br /&gt;&lt;br /&gt;&lt;b&gt;Valg af værksted&lt;/b&gt;&lt;br /&gt;Hvis din bil bliver skadet skal du benytte et GF Fordelsværksted,. Hvis du vælger ikke at gøre det, har du en ekstra selvrisiko på 1.113 kr. (2015). &lt;br /&gt;&lt;br /&gt;&lt;b&gt;Ekstra selvrisiko&lt;/b&gt;&lt;br /&gt;&lt;br /&gt;Får du en skade, som udløser selvrisiko, og som medfører udgift for GF Forsikring, gælder der for alle efterfølgende skader, som udløser selvrisiko og medfører udgift for GF Forsikring, en ekstra selvrisiko på 4.048 kr. (2015). Den ekstra selvrisiko er gældende i resten af det år skaden er sket og i det næste forsikringsår. (Forsikringsåret løber fra 1. januar til 31. december).";
                    //res.produkt.produktsamlerabat_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.&lt;br /&gt;&lt;br /&gt;Forudsætning for prisen på forsikringen er:&lt;br /&gt; &lt;ul&gt; &lt;li&gt;at du og andre brugere af bilen har lovbefalet kørekort.&lt;/li&gt; &lt;li&gt;at bilen ikke anvendes erhvervsmæssigt.&lt;/li&gt; &lt;li&gt;at der installeres satellit tracker, hvis bilen har en højere værdi end 646.125 kr. (2016).&lt;/li&gt; &lt;li&gt;at bilen ikke har over 260 hk.&lt;/li&gt; &lt;li&gt;at bilens handelsværdi ikke overstiger 753.813 kr. (2016).&lt;/li&gt;&lt;/ul&gt; Vær opmærksom på, at når du beregner din pris, går vi ud fra, at du har mindst én af følgende forsikringer hos os:&lt;br /&gt; &lt;br /&gt;Indbo-, hus-, ulykke-, sommerhus-, hunde- eller tingsforsikring.&lt;br /&gt; &lt;br /&gt; &lt;strong&gt;Valg af værksted&lt;/strong&gt; &lt;br /&gt;Hvis din bil bliver skadet skal du benytte et af de værksteder, som GF samarbejder med. Hvis du vælger ikke at gøre det, har du en ekstra selvrisiko på 1.128 kr. (2016).&lt;br /&gt; &lt;br /&gt; &lt;strong&gt;Ekstra selvrisiko&lt;/strong&gt; &lt;br /&gt;Får du en skade, som udløser selvrisiko, og som medfører udgift for GF Forsikring, gælder der for alle efterfølgende skader, som udløser selvrisiko og medfører udgift for GF Forsikring, en ekstra selvrisiko på 4.103 kr. (2016). Den ekstra selvrisiko på 4.103 kr. (2016) gælder ikke for skader omfattet af en eventuelt tilkøbt friskadedækning. Den ekstra selvrisiko er gældende i resten af det år skaden er sket og i det næste forsikringsår. (Forsikringsåret løber fra 1. januar til 31. december). ";
                    switch (req.affinity_no)
                    {
                        case 522:
                            res.produkt.produktpris_forudsaetninger = "&lt;p&gt;Prisen foruds&amp;aelig;tter, at de oplysninger du har afgivet i din prisberegning er korrekte. Du er bekendt med, at afgivelse af urigtige oplysninger kan medf&amp;oslash;re, at du ikke kan opn&amp;aring; den beregnede pris p&aring; forsikringsguiden.dk.&lt;/p&gt; &lt;ul&gt; &lt;li&gt;Den indtastede adresse er din folkeregisteradresse.&lt;/li&gt; &lt;li&gt;Indregistrering er sket med oplysning om CPR nummer.&lt;/li&gt; &lt;li&gt;Du er eller bliver registreret som bruger i Motorregisteret, der ikke er andre registrerede brugere end dig og eventuel &amp;aelig;gtef&amp;aelig;lle/samlever.&lt;/li&gt; &lt;li&gt;Bilen m&amp;aring; kun bruges til privat personk&amp;oslash;rsel.&lt;/li&gt; &lt;li&gt;Bilens handelsv&amp;aelig;rdi m&amp;aring; ikke overstige 1.000.000 kr. (2018).&lt;/li&gt; &lt;li&gt;Der skal installeres satellit tracker, hvis bilen har en h&amp;oslash;jere v&amp;aelig;rdi end 667.857 kr. (2018).&lt;/li&gt; &lt;li&gt;Bilen m&amp;aring; ikke have over 400 hk.&lt;/li&gt; &lt;li&gt;Bilen m&amp;aring; ikke v&amp;aelig;re ombygget.&lt;/li&gt; &lt;li&gt;Tvungen BS-betaling.&lt;/li&gt; &lt;li&gt;Tvungen e-boks&lt;/li&gt; &lt;li&gt;Tvungen hel&amp;aring;rlig betaling.&lt;/li&gt; &lt;li&gt;Du skal have bilforsikring i forvejen, eller have haft det i indev&amp;aelig;rende &amp;aring;r.&lt;/li&gt; &lt;li&gt;Du m&amp;aring; ikke have haft skader det sidste &amp;aring;r, og du skal have v&amp;aelig;ret bilkunde i nuv&amp;aelig;rende selskab i mindst 3 &amp;aring;r.&lt;/li&gt; &lt;li&gt;Du m&amp;aring; ikke v&amp;aelig;re registreret i RKI, Debitorregistret eller tilsvarende skyldnerregister.&lt;/li&gt; &lt;li&gt;Du m&amp;aring; ikke v&amp;aelig;re blevet opsagt eller p&amp;aring;lagt sk&amp;aelig;rpede vilk&amp;aring;r af andet selskab indenfor de sidste 2 &amp;aring;r&lt;/li&gt; &lt;li&gt;Du skal v&amp;aelig;re opm&amp;aelig;rksom p&amp;aring;, at vi for enkelte biltyper kan have krav om, at der indtegnes mindst en anden forsikringstype.?&lt;/li&gt; &lt;/ul&gt;";
                            res.produkt.produktsamlerabat_forudsaetninger = "&lt;p&gt;Prisen foruds&amp;aelig;tter, at de oplysninger du har afgivet i din prisberegning er korrekte. Du er bekendt med, at afgivelse af urigtige oplysninger kan medf&amp;oslash;re, at du ikke kan opn&amp;aring; den beregnede pris p&aring; forsikringsguiden.dk.&lt;/p&gt; &lt;ul&gt; &lt;li&gt;Den indtastede adresse er din folkeregisteradresse.&lt;/li&gt; &lt;li&gt;Indregistrering er sket med oplysning om CPR nummer.&lt;/li&gt; &lt;li&gt;Du er eller bliver registreret som bruger i Motorregisteret, der ikke er andre registrerede brugere end dig og eventuel &amp;aelig;gtef&amp;aelig;lle/samlever.&lt;/li&gt; &lt;li&gt;Bilen m&amp;aring; kun bruges til privat personk&amp;oslash;rsel.&lt;/li&gt; &lt;li&gt;Bilens handelsv&amp;aelig;rdi m&amp;aring; ikke overstige 1.000.000 kr. (2018).&lt;/li&gt; &lt;li&gt;Der skal installeres satellit tracker, hvis bilen har en h&amp;oslash;jere v&amp;aelig;rdi end 667.857 kr. (2018).&lt;/li&gt; &lt;li&gt;Bilen m&amp;aring; ikke have over 400 hk.&lt;/li&gt; &lt;li&gt;Bilen m&amp;aring; ikke v&amp;aelig;re ombygget.&lt;/li&gt; &lt;li&gt;Tvungen BS-betaling.&lt;/li&gt; &lt;li&gt;Tvungen e-boks&lt;/li&gt; &lt;li&gt;Tvungen hel&amp;aring;rlig betaling.&lt;/li&gt; &lt;li&gt;Du skal have bilforsikring i forvejen, eller have haft det i indev&amp;aelig;rende &amp;aring;r.&lt;/li&gt; &lt;li&gt;Du m&amp;aring; ikke have haft skader det sidste &amp;aring;r, og du skal have v&amp;aelig;ret bilkunde i nuv&amp;aelig;rende selskab i mindst 3 &amp;aring;r.&lt;/li&gt; &lt;li&gt;Du m&amp;aring; ikke v&amp;aelig;re registreret i RKI, Debitorregistret eller tilsvarende skyldnerregister.&lt;/li&gt; &lt;li&gt;Du m&amp;aring; ikke v&amp;aelig;re blevet opsagt eller p&amp;aring;lagt sk&amp;aelig;rpede vilk&amp;aring;r af andet selskab indenfor de sidste 2 &amp;aring;r&lt;/li&gt; &lt;li&gt;Du skal v&amp;aelig;re opm&amp;aelig;rksom p&amp;aring;, at vi for enkelte biltyper kan have krav om, at der indtegnes mindst en anden forsikringstype.?&lt;/li&gt; &lt;/ul&gt;";
                            break;
                        default:
                            res.produkt.produktpris_forudsaetninger = "&lt;p&gt;Prisen foruds&amp;aelig;tter, at de oplysninger du har afgivet i din prisberegning er korrekte. Du er bekendt med, at afgivelse af urigtige oplysninger kan medf&amp;oslash;re, at du ikke kan opn&amp;aring; den beregnede pris p&aring; forsikringsguiden.dk.&lt;/p&gt; &lt;ul&gt; &lt;li&gt;Den indtastede adresse er din folkeregisteradresse.&lt;/li&gt; &lt;li&gt;Indregistrering er sket med oplysning om CPR nummer.&lt;/li&gt; &lt;li&gt;Du er eller bliver registreret som bruger i Motorregisteret, der ikke er andre registrerede brugere end dig og eventuel &amp;aelig;gtef&amp;aelig;lle/samlever.&lt;/li&gt; &lt;li&gt;Bilen m&amp;aring; kun bruges til privat personk&amp;oslash;rsel.&lt;/li&gt; &lt;li&gt;Bilens handelsv&amp;aelig;rdi m&amp;aring; ikke overstige 1.000.000 kr. (2018).&lt;/li&gt; &lt;li&gt;Der skal installeres satellit tracker, hvis bilen har en h&amp;oslash;jere v&amp;aelig;rdi end 667.857 kr. (2018).&lt;/li&gt; &lt;li&gt;Bilen m&amp;aring; ikke have over 400 hk.&lt;/li&gt; &lt;li&gt;Bilen m&amp;aring; ikke v&amp;aelig;re ombygget.&lt;/li&gt; &lt;li&gt;Tvungen BS-betaling.&lt;/li&gt; &lt;li&gt;Tvungen e-boks&lt;/li&gt; &lt;li&gt;Tvungen hel&amp;aring;rlig betaling.&lt;/li&gt; &lt;li&gt;Du skal have bilforsikring i forvejen, eller have haft det i indev&amp;aelig;rende &amp;aring;r.&lt;/li&gt; &lt;li&gt;Du m&amp;aring; ikke have haft skader det sidste &amp;aring;r, og du skal have v&amp;aelig;ret bilkunde i nuv&amp;aelig;rende selskab i mindst 3 &amp;aring;r.&lt;/li&gt; &lt;li&gt;Du m&amp;aring; ikke v&amp;aelig;re registreret i RKI, Debitorregistret eller tilsvarende skyldnerregister.&lt;/li&gt; &lt;li&gt;Du m&amp;aring; ikke v&amp;aelig;re blevet opsagt eller p&amp;aring;lagt sk&amp;aelig;rpede vilk&amp;aring;r af andet selskab indenfor de sidste 2 &amp;aring;r&lt;/li&gt; &lt;li&gt;Du skal v&amp;aelig;re opm&amp;aelig;rksom p&amp;aring;, at vi for enkelte biltyper kan have krav om, at der indtegnes mindst en anden forsikringstype.?&lt;/li&gt; &lt;/ul&gt;";
                            res.produkt.produktsamlerabat_forudsaetninger = "&lt;p&gt;Prisen foruds&amp;aelig;tter, at de oplysninger du har afgivet i din prisberegning er korrekte. Du er bekendt med, at afgivelse af urigtige oplysninger kan medf&amp;oslash;re, at du ikke kan opn&amp;aring; den beregnede pris p&aring; forsikringsguiden.dk.&lt;/p&gt; &lt;ul&gt; &lt;li&gt;Den indtastede adresse er din folkeregisteradresse.&lt;/li&gt; &lt;li&gt;Indregistrering er sket med oplysning om CPR nummer.&lt;/li&gt; &lt;li&gt;Du er eller bliver registreret som bruger i Motorregisteret, der ikke er andre registrerede brugere end dig og eventuel &amp;aelig;gtef&amp;aelig;lle/samlever.&lt;/li&gt; &lt;li&gt;Bilen m&amp;aring; kun bruges til privat personk&amp;oslash;rsel.&lt;/li&gt; &lt;li&gt;Bilens handelsv&amp;aelig;rdi m&amp;aring; ikke overstige 1.000.000 kr. (2018).&lt;/li&gt; &lt;li&gt;Der skal installeres satellit tracker, hvis bilen har en h&amp;oslash;jere v&amp;aelig;rdi end 667.857 kr. (2018).&lt;/li&gt; &lt;li&gt;Bilen m&amp;aring; ikke have over 400 hk.&lt;/li&gt; &lt;li&gt;Bilen m&amp;aring; ikke v&amp;aelig;re ombygget.&lt;/li&gt; &lt;li&gt;Tvungen BS-betaling.&lt;/li&gt; &lt;li&gt;Tvungen e-boks&lt;/li&gt; &lt;li&gt;Tvungen hel&amp;aring;rlig betaling.&lt;/li&gt; &lt;li&gt;Du skal have bilforsikring i forvejen, eller have haft det i indev&amp;aelig;rende &amp;aring;r.&lt;/li&gt; &lt;li&gt;Du m&amp;aring; ikke have haft skader det sidste &amp;aring;r, og du skal have v&amp;aelig;ret bilkunde i nuv&amp;aelig;rende selskab i mindst 3 &amp;aring;r.&lt;/li&gt; &lt;li&gt;Du m&amp;aring; ikke v&amp;aelig;re registreret i RKI, Debitorregistret eller tilsvarende skyldnerregister.&lt;/li&gt; &lt;li&gt;Du m&amp;aring; ikke v&amp;aelig;re blevet opsagt eller p&amp;aring;lagt sk&amp;aelig;rpede vilk&amp;aring;r af andet selskab indenfor de sidste 2 &amp;aring;r&lt;/li&gt; &lt;li&gt;Du skal v&amp;aelig;re opm&amp;aelig;rksom p&amp;aring;, at vi for enkelte biltyper kan have krav om, at der indtegnes mindst en anden forsikringstype.?&lt;/li&gt; &lt;/ul&gt;";
                            break;
                    }
                    
                    if (TIARes.product[0].product_price != null && TIARes.product[0].error_message == "")
                    {
                        res.statuscode = 0;
                        res.statusdetail = "";
                        res.statusmessage = "";
                        res.ingenprisgrund.tilvalg1 = false;
                        res.ingenprisgrund.tilvalg2 = false;
                        res.ingenprisgrund.tilvalg3 = false;
                        res.ingenprisgrund.tilvalg4 = false;
                        res.ingenprisgrund.tilvalg5 = false;
                        res.ingenprisgrund.tilvalg6 = false;
                        res.ingenprisgrund.andet = false;
                        
                        //res.produkt.produktsamlerabat_forudsaetninger = "";
                        res.schemaVersion = "1.14M";

                        res.produkt.produktpris = (double)TIARes.product[0].product_price.discounted_price_yearly;
                        res.produkt.produktsamlerabat = res.produkt.produktpris; //motorkundepris fjernet på bil på version 15.
                        switch (req.affinity_no)
                        {
                            case 522: res.produkt.tilbagebetaling_af_praemie = false; break;
                            default: res.produkt.tilbagebetaling_af_praemie = true; break;
                        }
                        //res.bil.indplacering = "9";
                        res.bil.selvrisiko = (double)TIARes.product[0].product_line[0].@object[0].risks.risk[1].risk_excess;
                        //for (int risk = 0; risk < TIARes.product[0].product_line[0].@object[0].risks.risk.Length; risk++)
                        //{
                        //    switch (TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_name)
                        //    {
                        //        case "ansvar": res.bil.ansvar_kasko = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y" || res.bil.ansvar_kasko; break;
                        //        case "kasko": res.bil.ansvar_kasko = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y" || res.bil.ansvar_kasko; break;
                        //        case "foererulykke_doed": res.bil.foererdaekning = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y" || res.bil.foererdaekning; break;
                        //        case "foererulykke_varigt_men": res.bil.foererdaekning = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y" || res.bil.foererdaekning; break;
                        //        case "delkasko": break;
                        //        case "autoulykke_doed": break;
                        //        case "autoulykke_varigt_men": break;
                        //        case "friskade": res.bil.friskade = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                        //        case "bilafsavn": break;
                        //        case "vaerdiforringelse": break;
                        //        case "leasingpakke": break;
                        //        case "superelitebarn1": break;
                        //        case "superelitebarn2": break;
                        //        default: break;
                        //    }
                        //}

                        res.bil.ansvar_kasko = req.insurancerequest.bil.ansvar_kasko;
                        res.bil.fastpraemie = true;

                        if (req.insurancerequest.bil.vejhjaelp)
                        {
                            res.bil.vejhjaelp = true;
                        }

                        //if (res.produkt.produktnavn == "bilplus_gf")
                        //{
                        //    if (req.insurancerequest.bil.friskade || req.insurancerequest.bil.udvidetglas)
                        //    {
                        //        res.bil.friskade = true;
                        //        res.bil.udvidetglas = true;
                        //        res.bil.foererdaekning = true;
                        //    }

                        //    if (req.insurancerequest.bil.foererdaekning || req.insurancerequest.bil.vejhjaelp)
                        //    {
                        //        res.bil.foererdaekning = true;
                        //    }                            
                        //}

                        if (req.insurancerequest.bil.friskade)
                        {
                            res.bil.friskade = true;
                        }

                        if (req.insurancerequest.bil.foererdaekning || req.affinity_no == 522) //INTEGRA-636 (Forsikringsguiden - Førerulykke skal vises på forsikringsguiden på OK)
                        {
                            res.bil.foererdaekning = true;
                        }

                        if (req.insurancerequest.bil.udvidetglas)
                        {
                            res.bil.udvidetglas = true;
                        }

                        if (addGFHelp)
                        {
                            //The Road Assistance cannot be offered through the club, it must be added as the separate product.
                            if (PriceCalcConverter.ConvertGFHelpRequest(req, out gfTIAReq, out error))
                            {
                                if (client.CalculatePriceGF(gfTIAReq, out TIARes, out error))
                                {
                                    res.produkt.produktpris += (double)TIARes.product[0].product_price.price_yearly;
                                    res.produkt.produktsamlerabat += (double)(TIARes.product[0].product_price.price_yearly - TIARes.product[0].product_price.discounted_price_yearly);
                                }
                                else
                                {
                                    ErrorsLogger.Error(new ErrorInfo(error));
                                    return false;
                                }
                            }

                        }
                    }
                    else
                    {
                        res.statuscode = 1;
                        res.statusdetail = TIARes.product[0].error_message;
                        res.statusmessage = TIARes.product[0].error_message;
                    }
                }
                else
                {
                    error = exc.ToString();
                    ErrorsLogger.Error(new ErrorInfo(error));
                    return false;
                }
            }
            return true;
        }

        private bool CalculatePriceHU(string xmlRequest, out gfcalcResponse TIARes, out string error)
        {
            HS_request hsTIAReq = new HS_request();
            //HU_request huTIAReq = new HU_request();
            TIARes = null;
            ErrorInfo ei;
            //Calculate price for hus
            if (PriceCalcConverter.ConvertMicroHusRequest(req, out hsTIAReq, out error))
            //if (PriceCalcConverter.ConvertHusRequest(req, out huTIAReq, out error))
            {
                SerializationHelper.Serialize(hsTIAReq, out xmlRequest, out ei);
                //SerializationHelper.Serialize(huTIAReq, out xmlRequest, out ei);
                //F&P request successfully converted to GF request
                if (client.CalculatePriceHS(hsTIAReq, out TIARes, out error))
                //if (client.CalculatePriceHU(huTIAReq, out TIARes, out error))
                {
                    //We get positiv answer from TIA Price Calculator
                    res.hus = new responseHus();
                    res.ingenprisgrund = new responseIngenprisgrund();
                    switch (req.affinity_no)
                    {
                        case 522: res.produkt.produktnavn = "hus_ok"; break;
                        default: res.produkt.produktnavn = "hus_gf"; break;
                    }                    
                    if (TIARes.product[0].product_price != null 
                        && TIARes.product[0].error_message == "" 
                        && req.insurancerequest.hus.antal_skader_treaar < 3 
                        && req.insurancerequest.hus.tagbeklaedning != RoofingMaterialCodeType.Item7 
                        && !req.insurancerequest.hus.nedlagt_landbrug
                        && !(req.insurancerequest.hus.ydervaeg_kode == OuterWallsMaterialCodeType.Item4 && req.insurancerequest.hus.svampinsekt)
                        && !(req.insurancerequest.hus.opfoerelsesaar < 1901 && req.insurancerequest.hus.raad))
                    {
                        res.statuscode = 0;
                        res.statusdetail = "";
                        res.statusmessage = "";
                        res.ingenprisgrund.tilvalg1 = false;
                        res.ingenprisgrund.tilvalg2 = false;
                        res.ingenprisgrund.tilvalg3 = false;
                        res.ingenprisgrund.tilvalg4 = false;
                        res.ingenprisgrund.tilvalg5 = false;
                        res.ingenprisgrund.tilvalg6 = false;
                        res.ingenprisgrund.andet = false;

                        //res.produkt.produktpris_forudsaetninger = getProductAssumptions(res.produkt.produktnavn);
                        switch (req.affinity_no)
                        {
                            case 522:
                                res.produkt.produktpris_forudsaetninger = "&lt;p&gt;Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk. &lt;br/&gt;&lt;br/&gt;Forudsætning for prisen på forsikringen er; &lt;/p&gt;&lt;ul&gt;&lt;li&gt;at ejendommen ikke anvendes til erhvervsvirksomhed. &lt;/li&gt;&lt;li&gt;at ejendommen ikke anvendes til landbrug. &lt;/li&gt;&lt;li&gt;at ejendommen ikke er fredet. &lt;/li&gt;&lt;li&gt;at ejendommen ikke er tækket med strå. &lt;/li&gt;&lt;li&gt;at ejendommen ikke har halmfyr installeret. &lt;/li&gt;&lt;li&gt;at ejendommen ikke er under ombygning/tilbygning/opførelse &lt;/li&gt;&lt;/ul&gt;";
                                res.produkt.produktsamlerabat_forudsaetninger = "&lt;p&gt;Hvis du samler mindst tre af følgende forsikringer, får du 10 % samlerabat på dem: indbo-, hus-, ulykkes-, sommerhus-, kæledyrs-, knallert-, tings- og landboforsikring. Der gives ikke rabat på veteran-, MC- og campingvognsforsikring og på forsikringer, der tilbydes i samarbejde med OK Forsikrings samarbejdspartnere.&lt;/p&gt;";
                                break;
                            default:
                                res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.&lt;br&gt;&lt;br&gt;Forudsætning for prisen på forsikringen er;&lt;br&gt;&lt;ul&gt;&lt;li&gt;at ejendommen ikke anvendes til erhvervsvirksomhed.&lt;/li&gt;&lt;li&gt;at ejendommen ikke anvendes til landbrug.&lt;/li&gt;&lt;li&gt;at ejendommen ikke er fredet.&lt;/li&gt;&lt;li&gt;at ejendommen ikke er tækket med strå.&lt;/li&gt;&lt;li&gt;at ejendommen ikke har halmfyr installeret.&lt;/li&gt;&lt;li&gt;at ejendommen ikke er under ombygning/tilbygning/opførelse&lt;/li&gt;&lt;/ul&gt;";
                                res.produkt.produktsamlerabat_forudsaetninger = "Hvis du samler mindst tre af følgende forsikringer, får du 10 % samlerabat på dem: indbo-, hus-, ulykkes-, sommerhus-, hunde-, knallert- og tingsforsikring. Der gives ikke rabat på studie/uddannelse-, veteran-, MC- og campingvognsforsikring og på forsikringer, der tilbydes i samarbejde med GF's samarbejdspartnere.";
                                break;
                        }
                        res.schemaVersion = "1.14M";

                        res.produkt.produktpris = (double)TIARes.product[0].product_price.price_yearly;
                        res.produkt.produktsamlerabat = (double)TIARes.product[0].product_price.discounted_price_yearly;
                        res.produkt.tilbagebetaling_af_praemie = false;

                        res.hus.selvrisiko = (double)TIARes.product[0].product_line[0].@object[0].risks.risk[0].risk_excess;
                        for (int risk = 0; risk < TIARes.product[0].product_line[0].@object[0].risks.risk.Length; risk++)
                        {
                            /*switch (TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_name)
                            {
                                case "bygningsbrand": break;
                                case "bygningskasko": break;
                                case "svamp_insekt_og_raad": res.hus.svampinsekt = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "udvidet_roerskade": res.hus.roerkablerstikledninger = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; res.hus.kosmetiskdaekning = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "udv_roer_og_stikledning": res.hus.roerkablerstikledninger = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; res.hus.kosmetiskdaekning = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "stikledning": ; break;
                                case "udvidet_raad": res.hus.raad = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "udvidet_skadedyr": res.hus.dyrskader = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "skadesforsikringsafgift": ; break;
                                default: break;
                            }*/
                            switch (TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_name)
                            {
                                case "ildsvaade_lyn_eksplosion_mm": break;
                                case "kasko": break;
                                case "svamp_og_insekt": res.hus.svampinsekt = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "roer_og_kabel": res.hus.roerkablerstikledninger = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "stikledning": res.hus.roerkablerstikledninger = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "raad": res.hus.raad = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "elskade": break;
                                case "vejr_og_vand": break;
                                case "tyveri_og_haervaerk": break;
                                case "pludselig_skade": break;
                                case "glas_og_sanitet": break;
                                case "retshjaelp": break;
                                case "husejeransvar": break;
                                case "kosmetiske_forskelle": res.hus.kosmetiskdaekning = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "udvidet_vandskade": res.hus.udvidetvand = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "udvidet_daekning": res.hus.dyrskader = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "brand": break;
                                    
                                default: break;
                            }
                        }                        
                    }
                    else
                    {
                        res.statuscode = 1;
                        res.statusdetail = TIARes.product[0].error_message;
                        res.statusmessage = TIARes.product[0].error_message;
                        res.ingenprisgrund.tilvalg1 = false;
                        res.ingenprisgrund.tilvalg2 = false;
                        res.ingenprisgrund.tilvalg3 = false;
                        res.ingenprisgrund.tilvalg4 = false;
                        res.ingenprisgrund.tilvalg5 = false;
                        res.ingenprisgrund.tilvalg6 = false;
                        res.ingenprisgrund.andet = false;
                    }
                }
                else
                {
                    error = exc.ToString();
                    ErrorsLogger.Error(new ErrorInfo(error));
                    return false;
                }
            }
            else
            {
                res.statuscode = 1;
                res.statusdetail = error; // TIARes.product[0].error_message;
                res.statusmessage = error; // TIARes.product[0].error_message;
                res.ingenprisgrund = new responseIngenprisgrund();
                res.ingenprisgrund.tilvalg1 = false;
                res.ingenprisgrund.tilvalg2 = false;
                res.ingenprisgrund.tilvalg3 = false;
                res.ingenprisgrund.tilvalg4 = false;
                res.ingenprisgrund.tilvalg5 = false;
                res.ingenprisgrund.tilvalg6 = false;
                res.ingenprisgrund.andet = true;
            }
            return true;
        }

        private bool CalculatePriceSO(string xmlRequest, out gfcalcResponse TIARes, out string error)
        {
            //Calculate price for fritidshus
            SO_request soTIAReq = new SO_request();
            TIARes = null;
            //Calculate price for sos
            if (PriceCalcConverter.ConvertSommerhusRequest(req, out soTIAReq, out error))
            {
                //F&P request successfully converted to GF request
                if (client.CalculatePriceSO(soTIAReq, out TIARes, out error))
                {
                    //We get positiv answer from TIA Price Calculator
                    res.fritidshus = new responseFritidshus();
                    res.ingenprisgrund = new responseIngenprisgrund();
                    switch (req.affinity_no)
                    {
                        case 522: res.produkt.produktnavn = "sommerhus_ok"; break;
                        default: res.produkt.produktnavn = "sommerhus_gf"; break;
                    }                    
                    if (TIARes.product[0].product_price != null && TIARes.product[0].error_message == "" 
                        && req.insurancerequest.fritidshus.antal_skader_treaar < 3 
                        //&& !(req.insurancerequest.fritidshus.svampinsekt && (int)DateTime.Now.Year - (int)req.insurancerequest.fritidshus.opfoerelsesaar > 30) 
                        && !(req.insurancerequest.fritidshus.raad && (int)DateTime.Now.Year - (int)req.insurancerequest.fritidshus.opfoerelsesaar > 20) 
                        && req.insurancerequest.fritidshus.garage_m2 <= 75 
                        && req.insurancerequest.fritidshus.bebygget_m2 <= 200 
                        && req.insurancerequest.fritidshus.tagbeklaedning != RoofingMaterialCodeType.Item7 
                        && !req.insurancerequest.fritidshus.nedlagt_landbrug
                        && !req.insurancerequest.fritidshus.elektronik
                        && !req.insurancerequest.fritidshus.lejerskader
                        && !req.insurancerequest.fritidshus.kosmetiskdaekning)
                    {
                        res.statuscode = 0;
                        res.statusdetail = "";
                        res.statusmessage = "";
                        res.ingenprisgrund.tilvalg1 = false;
                        res.ingenprisgrund.tilvalg2 = false;
                        res.ingenprisgrund.tilvalg3 = false;
                        res.ingenprisgrund.tilvalg4 = false;
                        res.ingenprisgrund.tilvalg5 = false;
                        res.ingenprisgrund.tilvalg6 = false;
                        res.ingenprisgrund.andet = false;

                        //res.produkt.produktpris_forudsaetninger = getProductAssumptions(res.produkt.produktnavn);
                        switch (req.affinity_no)
                        {
                            case 522:
                                res.produkt.produktpris_forudsaetninger = "&lt;p&gt;Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk. &lt;br/&gt;&lt;br/&gt;Forudsætning for prisen på forsikringen er; &lt;/p&gt;&lt;ul&gt;&lt;li&gt;at ejendommen ikke er et kolonihavehus &lt;/li&gt;&lt;li&gt;at ejendommen ikke er et nedlagt landbrug &lt;/li&gt;&lt;li&gt;at ejendommen ikke er fredet eller bevaringsværdigt &lt;/li&gt;&lt;li&gt;at der ikke drives erhverv i ejendommen &lt;/li&gt;&lt;li&gt;at der ikke er stråtag/blødt tag på ejendommen &lt;/li&gt;&lt;li&gt;at ejendommen ikke er et halmhus &lt;/li&gt;&lt;li&gt;at ejendommen ikke er under opførsel, ombygning eller renovering &lt;/li&gt;&lt;li&gt;at ejendomme ikke er et bjælkehus, et pudset træhus eller et skalmuret træhus hvis der er valgt svamp og insekt &lt;/li&gt;&lt;li&gt;at ejendommen skal besigtiges hvis der er valgt svamp og insekt og den er ældre end 30 år &lt;/li&gt;&lt;li&gt;svamp- og rådskade dækkes ikke i og i forbindelse med stern, vindskeder og udhæng på ejendomme med tag af græstørv &lt;/li&gt;&lt;/ul&gt;";
                                res.produkt.produktsamlerabat_forudsaetninger = "&lt;p&gt; Hvis du samler mindst tre af følgende forsikringer, får du 10 % samlerabat på dem: indbo-, hus-, ulykkes-, sommerhus-, kæledyrs-, knallert-, tings- og landboforsikring. Der gives ikke rabat på veteran-, MC- og campingvognsforsikring og på forsikringer, der tilbydes i samarbejde med OK Forsikrings samarbejdspartnere. &lt;/p&gt;";
                                break;
                            default:
                                res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.&lt;br&gt;&lt;br&gt;Forudsætning for prisen på forsikringen er;&lt;br&gt;&lt;ul&gt;&lt;li&gt;at ejendommen ikke er et kolonihavehus&lt;/li&gt;&lt;li&gt;at ejendommen ikke er et nedlagt landbrug&lt;/li&gt;&lt;li&gt;at ejendommen ikke er fredet eller bevaringsværdigt&lt;/li&gt;&lt;li&gt;at der ikke drives erhverv i ejendommen&lt;/li&gt;&lt;li&gt;at der ikke er stråtag/blødt tag på ejendommen&lt;/li&gt;&lt;li&gt;at ejendommen ikke er et halmhus&lt;/li&gt;&lt;li&gt;at ejendommen ikke er under opførsel, ombygning eller renovering&lt;/li&gt;&lt;li&gt;at ejendomme ikke er et bjælkehus,  et pudset træhus eller et skalmuret træhus hvis der er valgt svamp og insekt&lt;/li&gt;&lt;li&gt;at ejendommen skal besigtiges hvis der er valgt svamp og insekt og den er ældre end 30 år&lt;/li&gt;&lt;li&gt;svamp- og rådskade dækkes ikke i og i forbindelse med stern, vindskeder og udhæng på ejendomme med tag af græstørv&lt;/li&gt;&lt;ul&gt;&lt;br&gt;";
                                res.produkt.produktsamlerabat_forudsaetninger = "Hvis du samler mindst tre af følgende forsikringer, får du 10 % samlerabat på dem: indbo-, hus-, ulykkes-, sommerhus-, hunde-, knallert- og tingsforsikring. Der gives ikke rabat på studie/uddannelse-, veteran-, MC- og campingvognsforsikring og på forsikringer, der tilbydes i samarbejde med GF's samarbejdspartnere.";
                                break;
                        }
                        
                        res.schemaVersion = "1.14M";

                        res.produkt.produktpris = (double)TIARes.product[0].product_price.price_yearly;
                        res.produkt.produktsamlerabat = (double)TIARes.product[0].product_price.discounted_price_yearly;
                        res.produkt.tilbagebetaling_af_praemie = false;

                        res.fritidshus.selvrisiko = (double)TIARes.product[0].product_line[0].@object[0].risks.risk[0].risk_excess;
                        for (int risk = 0; risk < TIARes.product[0].product_line[0].@object[0].risks.risk.Length; risk++)
                        {
                            switch (TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_name)
                            {
                                case "bygningsbrand": break;
                                case "bygningskasko": break;
                                case "indbo": res.fritidshus.sum = (double)TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_sum; break;
                                case "el-skade": break;
                                case "raad": res.fritidshus.raad = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "svamp_og_insekt": res.fritidshus.svampinsekt = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "udvidet_roerskade": res.fritidshus.roerstikledninger = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                case "stikledning": break;
                                default: break;

                            }
                        }
                    }
                    else
                    {
                        res.statuscode = 1;
                        res.statusdetail = TIARes.product[0].error_message;
                        res.statusmessage = TIARes.product[0].error_message;
                    }
                }
                else
                {
                    error = exc.ToString();
                    ErrorsLogger.Error(new ErrorInfo(error));                    
                    return false;
                }
            }
            return true;
        }

        private bool CalculatePriceUL(string xmlRequest, out gfcalcResponse TIARes, out string error)
        {
            //Calculate price for ulykke
            int object_id = 0;
            UL_request ulTIAReq = new UL_request();
            TIARes = null;
            if (PriceCalcConverter.ConvertUlykkeRequest(req, out ulTIAReq, out error))
            {
                //F&P request successfully converted to GF request
                if (client.CalculatePriceUL(ulTIAReq, out TIARes, out error))
                {
                    //We get positiv answer from TIA Price Calculator
                    res.ulykke = new responseUlykke();
                    res.ingenprisgrund = new responseIngenprisgrund();
                    //res.produkt.produktnavn = "Ulykkesforsikring";

                    if (req.insurancerequest.ulykke.boern != null)
                    {
                        //Med børn
                        if (req.insurancerequest.ulykke.samlever != null)
                        {
                            //Familie
                            if (req.insurancerequest.ulykke.heltidsulykke == true && req.insurancerequest.ulykke.samlever.heltidsulykke == true)
                            {
                                switch (req.affinity_no)
                                {
                                    case 522: res.produkt.produktnavn = "familie_heltid_heltid_barn_ok"; break;
                                    default: res.produkt.produktnavn = "familie_heltid_heltid_barn_gf"; break;
                                }                                
                                //res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.";
                            }
                            else if (req.insurancerequest.ulykke.heltidsulykke == false && req.insurancerequest.ulykke.samlever.heltidsulykke == false)
                            {
                                switch (req.affinity_no)
                                {
                                    case 522: res.produkt.produktnavn = "familie_fritid_fritid_barn_ok"; break;
                                    default: res.produkt.produktnavn = "familie_fritid_fritid_barn_gf"; break;
                                }                                
                                //res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.";
                            }
                            else if (((req.insurancerequest.ulykke.heltidsulykke == false && req.insurancerequest.ulykke.samlever.heltidsulykke == true) || (req.insurancerequest.ulykke.heltidsulykke == true && req.insurancerequest.ulykke.samlever.heltidsulykke == false)) && req.insurancerequest.ulykke.boern.antal_u18 > 0)
                            {
                                switch (req.affinity_no)
                                {
                                    case 522: res.produkt.produktnavn = "familie_fritid_heltid_barn_ok"; break;
                                    default: res.produkt.produktnavn = "familie_fritid_heltid_barn_gf"; break;
                                }
                                //res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.";
                            }
                        }
                        else
                        {
                            //Enkelt
                            if (req.insurancerequest.ulykke.heltidsulykke == true)
                            {
                                switch (req.affinity_no)
                                {
                                    case 522: res.produkt.produktnavn = "enkelt_heltid_barn_ok"; break;
                                    default: res.produkt.produktnavn = "enkelt_heltid_barn_gf"; break;
                                }
                                //res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.";
                            }
                            else if (req.insurancerequest.ulykke.heltidsulykke == false)
                            {
                                switch (req.affinity_no)
                                {
                                    case 522: res.produkt.produktnavn = "enkelt_fritid_barn_ok"; break;
                                    default: res.produkt.produktnavn = "enkelt_fritid_barn_gf"; break;
                                }                                
                                //res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.";
                            }
                        }
                    }
                    else
                    {
                        //Uden børn
                        if (req.insurancerequest.ulykke.samlever != null)
                        {
                            //Familie
                            if (req.insurancerequest.ulykke.heltidsulykke == true && req.insurancerequest.ulykke.samlever.heltidsulykke == true)
                            {
                                switch (req.affinity_no)
                                {
                                    case 522: res.produkt.produktnavn = "familie_heltid_heltid_ok"; break;
                                    default: res.produkt.produktnavn = "familie_heltid_heltid_gf"; break;
                                }
                                //res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.";
                            }
                            else if (req.insurancerequest.ulykke.heltidsulykke == false && req.insurancerequest.ulykke.samlever.heltidsulykke == false)
                            {
                                switch (req.affinity_no)
                                {
                                    case 522: res.produkt.produktnavn = "familie_fritid_fritid_ok"; break;
                                    default: res.produkt.produktnavn = "familie_fritid_fritid_gf"; break;
                                }                                
                                //res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.";
                            }
                            else if (((req.insurancerequest.ulykke.heltidsulykke == false && req.insurancerequest.ulykke.samlever.heltidsulykke == true) || (req.insurancerequest.ulykke.heltidsulykke == true && req.insurancerequest.ulykke.samlever.heltidsulykke == false)))
                            {
                                switch (req.affinity_no)
                                {
                                    case 522: res.produkt.produktnavn = "familie_fritid_heltid_ok"; break;
                                    default: res.produkt.produktnavn = "familie_fritid_heltid_gf"; break;
                                }
                                //res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.";
                            }
                        }
                        else
                        {
                            //Enkelt
                            if (req.insurancerequest.ulykke.heltidsulykke == true)
                            {
                                switch (req.affinity_no)
                                {
                                    case 522: res.produkt.produktnavn = "enkelt_heltid_ok"; break;
                                    default: res.produkt.produktnavn = "enkelt_heltid_gf"; break;
                                }
                                //res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.";
                            }
                            else if (req.insurancerequest.ulykke.heltidsulykke == false)
                            {
                                switch (req.affinity_no)
                                {
                                    case 522: res.produkt.produktnavn = "enkelt_fritid_ok"; break;
                                    default: res.produkt.produktnavn = "enkelt_fritid_gf"; break;
                                }
                                //res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.";
                            }
                        }
                    }

                    if (TIARes.product[0].product_price != null && TIARes.product[0].error_message == "")
                    {
                        res.statuscode = 0;
                        res.statusdetail = "";
                        res.statusmessage = "";
                        res.ingenprisgrund.tilvalg1 = false;
                        res.ingenprisgrund.tilvalg2 = false;
                        res.ingenprisgrund.tilvalg3 = false;
                        res.ingenprisgrund.tilvalg4 = false;
                        res.ingenprisgrund.tilvalg5 = false;
                        res.ingenprisgrund.tilvalg6 = false;
                        res.ingenprisgrund.andet = false;

                        //res.produkt.produktpris_forudsaetninger = getProductAssumptions(res.produkt.produktnavn);
                        switch (req.affinity_no)
                        {
                            case 522:
                                res.produkt.produktpris_forudsaetninger = "&lt;p&gt;Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.&lt;/p&gt;";
                                res.produkt.produktsamlerabat_forudsaetninger = "&lt;p&gt;Hvis du samler mindst tre af følgende forsikringer, får du 10 % samlerabat på dem: indbo -, hus -, ulykkes -, sommerhus -, kæledyrs -, knallert -, tings - og landboforsikring.Der gives ikke rabat på veteran-, MC - og campingvognsforsikring og på forsikringer, der tilbydes i samarbejde med OK Forsikrings samarbejdspartnere.&lt;/p&gt;";
                                break;
                            default:
                                res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.";
                                res.produkt.produktsamlerabat_forudsaetninger = "Hvis du samler mindst tre af følgende forsikringer, får du 10 % samlerabat på dem: indbo-, hus-, ulykkes-, sommerhus-, hunde-, knallert- og tingsforsikring. Der gives ikke rabat på studie/uddannelse-, veteran-, MC- og campingvognsforsikring og på forsikringer, der tilbydes i samarbejde med GF's samarbejdspartnere.";
                                break;
                        }                        
                        res.schemaVersion = "1.14M";

                        res.produkt.produktpris = (double)TIARes.product[0].product_price.price_yearly;
                        res.produkt.produktsamlerabat = (double)TIARes.product[0].product_price.discounted_price_yearly;
                        res.produkt.tilbagebetaling_af_praemie = false;

                        res.ulykke.produktpris_dig = (double)TIARes.product[0].product_line[0].@object[object_id].object_price.price_yearly;
                        for (int i = 0; i < TIARes.product[0].product_line[0].@object[object_id].risks.risk.Length; i++)
                        {
                            switch (TIARes.product[0].product_line[0].@object[object_id].risks.risk[i].risk_name)
                            {
                                case "doed": res.ulykke.sum_doed_dig = (double)TIARes.product[0].product_line[0].@object[object_id].risks.risk[i].risk_sum; break;
                                case "varigt_men": res.ulykke.sum_varigt_men_dig = (double)TIARes.product[0].product_line[0].@object[object_id].risks.risk[i].risk_sum; break;
                                default: break;
                            }
                        }
                        object_id++;

                        if (req.insurancerequest.ulykke.samlever != null && TIARes.product[0].product_line[0].@object.Length > 1)
                        {
                            res.ulykke.produktpris_samlever = (double)TIARes.product[0].product_line[0].@object[object_id].object_price.price_yearly;
                            for (int i = 0; i < TIARes.product[0].product_line[0].@object[object_id].risks.risk.Length; i++)
                            {
                                switch (TIARes.product[0].product_line[0].@object[object_id].risks.risk[i].risk_name)
                                {
                                    case "doed": res.ulykke.sum_doed_samlever = (double)TIARes.product[0].product_line[0].@object[object_id].risks.risk[i].risk_sum; break;
                                    case "varigt_men": res.ulykke.sum_varigt_men_samlever = (double)TIARes.product[0].product_line[0].@object[object_id].risks.risk[i].risk_sum; break;

                                    default: break;
                                }
                            }
                            object_id++;
                        }
                        
                        if (req.insurancerequest.ulykke.boern != null && TIARes.product[0].product_line[0].@object.Length > 1)
                        {
                            //The price for children should be summarized rather than price per child
                            //res.ulykke.produktpris_born = (double)TIARes.product[0].product_line[0].@object[object_id].object_price.price_yearly;
                            while (object_id < TIARes.product[0].product_line[0].@object.Length)
                            {
                                res.ulykke.produktpris_born += (double)TIARes.product[0].product_line[0].@object[object_id].object_price.price_yearly;
                                object_id++;
                            }                            
                            for (int i = 0; i < TIARes.product[0].product_line[0].@object[TIARes.product[0].product_line[0].@object.Length-1].risks.risk.Length; i++)
                            {
                                switch (TIARes.product[0].product_line[0].@object[TIARes.product[0].product_line[0].@object.Length - 1].risks.risk[i].risk_name)
                                {
                                    case "varigt_men": res.ulykke.sum_varigt_men_born = (double)TIARes.product[0].product_line[0].@object[TIARes.product[0].product_line[0].@object.Length - 1].risks.risk[i].risk_sum; break;
                                    default: break;
                                }
                            }
                            // Removed beause of new schema
                            //res.ulykke.boerneulykke = true;
                        }                        
                        //If requested a farligsport for any person in request, farligsport must be marked in the answer with the truth
                        if (req.insurancerequest.ulykke.farligsport || (req.insurancerequest.ulykke.samlever != null && req.insurancerequest.ulykke.samlever.farligsport) || (req.insurancerequest.ulykke.boern != null && req.insurancerequest.ulykke.boern.farligsport))
                        {
                            res.ulykke.farligsport = true;
                        }
                        //dobbelterstatning must be marked as true in response independent what is requested
                        res.ulykke.dobbelterstatning = true;
                        //motorcykelknallert must be marked as true in response independent what is requested
                        res.ulykke.motorcykelknallert = true;
                        //strakserstatning must be marked as false in response independent what is requested
                        res.ulykke.strakserstatning = false;

                        if (req.insurancerequest.ulykke.forsikringssum_doed + req.insurancerequest.ulykke.forsikringssum_varigt_men > ULSumMax) //INTEGRA-656 Ulykkessummer på forsikringsguiden
                        {
                            res.statuscode = 1;
                            //res.statusdetail = TIARes.product[0].error_message;
                            res.statusmessage = "Den samlede forsikringssum må ikke overstige kr. " + ULSumMax + ".";
                        }
                    }
                    else
                    {
                        res.statuscode = 1;
                        res.statusdetail = TIARes.product[0].error_message;
                        res.statusmessage = TIARes.product[0].error_message;
                    }
                }
                else
                {
                    error = exc.ToString();
                    ErrorsLogger.Error(new ErrorInfo(error));
                    return false;
                }
            }
            else
            {
                //Converterings process failed
                res.ulykke = new responseUlykke();
                res.ingenprisgrund = new responseIngenprisgrund();
                res.statuscode = 1;
                res.statusdetail = error;
                res.statusmessage = error;
                return false;
            }
            return true;
        }

        private bool CalculatePriceIN(string xmlRequest, out gfcalcResponse TIARes, out string error)
        {
            //Calculate price for indbo
            IN_request inTIAReq = new IN_request();
            TIARes = null;

            if (req.insurancerequest.indbo.antal_boern < 6 && req.insurancerequest.indbo.antal_voksne < 5)
            {
                if (PriceCalcConverter.ConvertIndboRequest(req, out inTIAReq, out error))
                {
                    //F&P request successfully converted to GF request
                    if (client.CalculatePriceIN(inTIAReq, out TIARes, out error))
                    {
                        //We get positiv answer from TIA Price Calculator
                        res.indbo = new responseIndbo();
                        res.ingenprisgrund = new responseIngenprisgrund();
                        switch (req.affinity_no)
                        {
                            case 522: res.produkt.produktnavn = "indbo_ok"; break;
                            default: res.produkt.produktnavn = "indbo_gf"; break;
                        }                        
                        if (TIARes.product[0].product_price != null && TIARes.product[0].error_message == "" && req.insurancerequest.indbo.antal_skader_treaar < 3) //Checking answer content + amount of damages
                        {
                            res.statuscode = 0;
                            res.statusdetail = "";
                            res.statusmessage = "";
                            res.ingenprisgrund.tilvalg1 = false;
                            res.ingenprisgrund.tilvalg2 = false;
                            res.ingenprisgrund.tilvalg3 = false;
                            res.ingenprisgrund.tilvalg4 = false;
                            res.ingenprisgrund.tilvalg5 = false;
                            res.ingenprisgrund.tilvalg6 = false;
                            res.ingenprisgrund.andet = false;

                            //res.produkt.produktpris_forudsaetninger = getProductAssumptions(res.produkt.produktnavn);
                            switch (req.affinity_no)
                            {
                                case 522:
                                    res.produkt.produktpris_forudsaetninger = "&lt;p&gt;Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk. &lt;br/&gt;&lt;br/&gt;Forudsætning for prisen på forsikringen er; &lt;/p&gt;&lt;ul&gt;&lt;li&gt;at ejendommen ikke anvendes til brandfarlig bedrift. &lt;/li&gt;&lt;li&gt;at ejendommen ikke er en landbrugsejendom. &lt;/li&gt;&lt;li&gt;at ejendommen ikke er tækket med strå. &lt;/li&gt;&lt;/ul&gt;";
                                    res.produkt.produktsamlerabat_forudsaetninger = "&lt;p&gt;Hvis du samler mindst tre af følgende forsikringer, får du 10 % samlerabat på dem: indbo-, hus-, ulykkes-, sommerhus-, kæledyrs-, knallert-, tings- og landboforsikring. Der gives ikke rabat på veteran-, MC- og campingvognsforsikring og på forsikringer, der tilbydes i samarbejde med OK Forsikrings samarbejdspartnere.&lt;/p&gt;";
                                    break;
                                default:
                                    res.produkt.produktpris_forudsaetninger = "Prisen forudsætter, at de oplysninger du har afgivet i din prisberegning er korrekte. Endvidere forudsættes at du eller din husstand aldrig er blevet pålagt strengere vilkår (fx forhøjet selvrisiko eller præmie) eller aldrig er blevet opsagt af noget selskab tidligere pga. skader, restance mv. Du er bekendt med, at afgivelse af urigtige oplysninger kan medføre, at du ikke kan opnå den beregnede pris på forsikringsguiden.dk.&lt;br&gt;&lt;br&gt;Forudsætning for prisen på forsikringen er;&lt;br&gt;&lt;ul&gt;&lt;li&gt;at ejendommen ikke anvendes til brandfarlig bedrift. &lt;/li&gt;&lt;li&gt;at ejendommen ikke er en landbrugsejendom. &lt;/li&gt;&lt;li&gt;at ejendommen ikke er tækket med strå. &lt;/li&gt;&lt;/ul&gt;";
                                    res.produkt.produktsamlerabat_forudsaetninger = "Hvis du samler mindst tre af følgende forsikringer, får du 10 % samlerabat på dem: indbo-, hus-, ulykkes-, sommerhus-, hunde-, knallert- og tingsforsikring. Der gives ikke rabat på studie/uddannelse-, veteran-, MC- og campingvognsforsikring og på forsikringer, der tilbydes i samarbejde med GF's samarbejdspartnere.";
                                    break;
                            }
                            res.schemaVersion = "1.14M";

                            res.produkt.produktpris = (double)TIARes.product[0].product_price.price_yearly;
                            res.produkt.produktsamlerabat = (double)TIARes.product[0].product_price.discounted_price_yearly;
                            res.produkt.tilbagebetaling_af_praemie = false;
                            res.indbo.sum = (double)TIARes.product[0].product_line[0].@object[0].risks.risk[0].risk_sum;
                            res.indbo.selvrisiko = (double)TIARes.product[0].product_line[0].@object[0].risks.risk[0].risk_excess;
                            for (int risk = 0; risk < TIARes.product[0].product_line[0].@object[0].risks.risk.Length; risk++)
                            {
                                switch (TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_name)
                                {
                                    case "glas_og_sanitet": res.indbo.glaskumme = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                    case "pludselig_skade": res.indbo.pludseligskade = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                    case "elektronikforsikring": res.indbo.elektronik = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                    case "rejse_-_europa_inkl_tyrkiet": res.indbo.rejseeuropa = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                    case "rejse_-_verden": res.indbo.rejseverden = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                    case "rejse_-_afbestilling": res.indbo.afbestillingsforsikring = TIARes.product[0].product_line[0].@object[0].risks.risk[risk].risk_yn == "Y"; break;
                                    default: break;
                                }
                            }
                        }
                        else
                        {
                            res.statuscode = 1;
                            res.statusdetail = TIARes.product[0].error_message;
                            res.statusmessage = TIARes.product[0].error_message;
                        }

                    }
                    else
                    {
                        error = exc.ToString();
                        ErrorsLogger.Error(new ErrorInfo(error));
                        return false;
                    }
                }
                else
                {
                    //Converterings process failed
                    res.indbo = new responseIndbo();
                    res.ingenprisgrund = new responseIngenprisgrund();
                    res.statuscode = 1;
                    res.statusdetail = error;
                    res.statusmessage = error;
                    return false;
                }
            }
            else
            {
                //Preconditions failed
                res.indbo = new responseIndbo();
                res.ingenprisgrund = new responseIngenprisgrund();
                res.statuscode = 1;
                res.statusdetail = "Ingen match";
                res.statusmessage = "Ingen match";
                error = "Ingen match";
                return false;
            }
            return true;
        }

        private string getErrorMessage(string errrorCode)
        {
            try
            {
                return ConfigurationManager.AppSettings[errrorCode];
            }
            catch (Exception exc)
            {
                ErrorsLogger.Error(new ErrorInfo(exc.ToString()));
                return null;
            }            
        }

        //private string getProductAssumptions(string productName)
        //{
        //    try
        //    {
        //        return ConfigurationManager.AppSettings[productName].Replace("&lt;", "<").Replace("&gt;", ">");                
        //    }
        //    catch (Exception exc)
        //    {
        //        PriceCalcLogger.Log(new ErrorInfo(exc.ToString()));
        //        return null;
        //    }
        //}
        #endregion Private Methods
    }
}
