using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient;
//using UnitTest.FandP.PriceCalc.ReferenceServiceClient;

using System.Xml;


namespace UnitTest.FandP.PriceCalc
{
    /// <summary>
    /// Summary description for UnitTestProdHU
    /// </summary>
    [TestClass]
    public class UnitTestProdHU
    {
        /*
         * Artifact artf290436 : Huse med stråtag får pris eller teknisk fejl vises 
         *         
         * Indtast adresse:  Kukkenbjerg 28, Svanninge, 5600 Faaborg. Her vises GF med teknisk fejl.
         * Indtast: Lærkealle 2, Lodbj Hede, 6950. Her beregnes en pris.
         * Se vedhæftede screenshot.
        */

        [TestMethod]
        public void TestHUProduct01()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>43</alder><postnr>5600</postnr><adresse>Kukkenbjerg 28, Svanninge, 5600 Faaborg</adresse><kvhx>43007810028000000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>f29f9d8f-278a-4817-89a6-60dec25141ba</sessionid><insurancerequest><hus><opfoerelsesaar>1877</opfoerelsesaar><bebygget_m2>190</bebygget_m2><bolig_m2>190</bolig_m2><garage_m2>0</garage_m2><etager>1</etager><tagbeklaedning>7</tagbeklaedning><kaelder_m2>0</kaelder_m2><antal_skader_treaar>0</antal_skader_treaar><opvarmning>1</opvarmning><pool_m2>0</pool_m2><pool_placering>indendoers</pool_placering><vandstop>false</vandstop><hoejvandslukke>false</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>1</toiletbadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>false</svampinsekt><raad>false</raad><roerkablerstikledninger>false</roerkablerstikledninger><selvrisiko>3000</selvrisiko><bo_type>120</bo_type><ydervaeg_kode>4</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>7</varmeinstallation><erhverv_m2>0</erhverv_m2><lejligheder>1</lejligheder><overdaek_m2>0</overdaek_m2><tagetage_lovlig_m2>0</tagetage_lovlig_m2></hus></insurancerequest></request>";                
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));            
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        /*
         * Artifact artf290445 : Der beregnes for Nedlagt Landbrug
         * 
         * Ret oplysninger på Bramgåsvej 2A, 4760 til nedlagt landbrug.
        */

        [TestMethod]
        public void TestHUProduct02()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>43</alder><postnr>4760</postnr><adresse>Bramgåsvej 2A, 4760 Vordingborg</adresse><kvhx>3900160002A000000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>f29f9d8f-278a-4817-89a6-60dec25141ba</sessionid><insurancerequest><hus><opfoerelsesaar>2006</opfoerelsesaar><bebygget_m2>86</bebygget_m2><bolig_m2>80</bolig_m2><garage_m2>6</garage_m2><etager>1</etager><tagbeklaedning>2</tagbeklaedning><kaelder_m2>0</kaelder_m2><antal_skader_treaar>0</antal_skader_treaar><opvarmning>1</opvarmning><pool_m2>0</pool_m2><pool_placering>indendoers</pool_placering><vandstop>false</vandstop><hoejvandslukke>false</hoejvandslukke><nedlagt_landbrug>true</nedlagt_landbrug><toiletbadrum>1</toiletbadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>true</svampinsekt><raad>false</raad><roerkablerstikledninger>true</roerkablerstikledninger><selvrisiko>2000</selvrisiko><bo_type>510</bo_type><ydervaeg_kode>5</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>7</varmeinstallation><erhverv_m2>0</erhverv_m2><lejligheder>0</lejligheder><overdaek_m2>10</overdaek_m2><tagetage_lovlig_m2>0</tagetage_lovlig_m2></hus></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));            
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        
        [TestMethod]
        public void TestHUProduct03()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>35</alder><postnr>5400</postnr><adresse>Hasselvej 4, 5400 Bogense</adresse><kvhx>48004410004000000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>be83a7b8-4318-4d24-af97-9ccecb1cc77e</sessionid><insurancerequest><hus><opfoerelsesaar>1976</opfoerelsesaar><bebygget_m2>145</bebygget_m2><bolig_m2>145</bolig_m2><garage_m2>28</garage_m2><etager>1</etager><tagbeklaedning>5</tagbeklaedning><kaelder_m2>40</kaelder_m2><antal_skader_treaar>0</antal_skader_treaar><opvarmning>51</opvarmning><pool_m2>0</pool_m2><pool_placering>indendoers</pool_placering><vandstop>false</vandstop><hoejvandslukke>false</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>false</svampinsekt><raad>false</raad><roerkablerstikledninger>false</roerkablerstikledninger><selvrisiko>3000</selvrisiko><bo_type>120</bo_type><ydervaeg_kode>1</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>1</varmeinstallation><erhverv_m2>0</erhverv_m2><lejligheder>1</lejligheder><overdaek_m2>0</overdaek_m2><tagetage_lovlig_m2>0</tagetage_lovlig_m2></hus></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));            
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestHUProduct04()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>40</alder><postnr>4534</postnr><adresse>Kirkestien 5, Vallekilde, 4534 Hørve</adresse><kvhx>30611500005000000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>935e1eda-f5df-4e2d-826d-7180292fe9bb</sessionid><insurancerequest><hus><opfoerelsesaar>1905</opfoerelsesaar><bebygget_m2>123</bebygget_m2><bolig_m2>198</bolig_m2><garage_m2>15</garage_m2><etager>2</etager><tagbeklaedning>3</tagbeklaedning><kaelder_m2>80</kaelder_m2><antal_skader_treaar>0</antal_skader_treaar><opvarmning>7</opvarmning><pool_m2>0</pool_m2><pool_placering>indendoers</pool_placering><vandstop>false</vandstop><hoejvandslukke>false</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>true</svampinsekt><raad>false</raad><roerkablerstikledninger>true</roerkablerstikledninger><selvrisiko>2000</selvrisiko><bo_type>120</bo_type><ydervaeg_kode>4</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>2</varmeinstallation><erhverv_m2>0</erhverv_m2><lejligheder>1</lejligheder><overdaek_m2>0</overdaek_m2><tagetage_lovlig_m2>75</tagetage_lovlig_m2></hus></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestHSProduct05()
        {
            string xmlRequest = "<?xml version=\"1.0\" encoding=\"utf-8\"?><request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>44</alder><postnr>3550</postnr><adresse>Kongensgade 11, 3550 Slangerup</adresse><kvhx>25006510011000000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>424eb99a-3642-4254-bbc4-59eaf303116f</sessionid><insurancerequest><hus><opfoerelsesaar>1924</opfoerelsesaar><bebygget_m2>140</bebygget_m2><bolig_m2>344</bolig_m2><garage_m2>0</garage_m2><etager>3</etager><tagbeklaedning>5</tagbeklaedning><kaelder_m2>30</kaelder_m2><antal_skader_treaar>0</antal_skader_treaar><opvarmning>7</opvarmning><pool_m2>0</pool_m2><pool_placering>indendoers</pool_placering><vandstop>false</vandstop><hoejvandslukke>false</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>1</toiletbadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>true</svampinsekt><raad>false</raad><roerkablerstikledninger>true</roerkablerstikledninger><selvrisiko>2000</selvrisiko><bo_type>140</bo_type><ydervaeg_kode>1</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>2</varmeinstallation><erhverv_m2>140</erhverv_m2><lejligheder>1</lejligheder><overdaek_m2>39</overdaek_m2><tagetage_lovlig_m2>70</tagetage_lovlig_m2></hus></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }


    }
}
