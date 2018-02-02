using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient;
//using UnitTest.FandP.PriceCalc.ReferenceServiceClient;

using System.Xml;

namespace UnitTest.FandP.PriceCalc
{
    [TestClass]
    public class UnitTestProdUL
    {


        [TestMethod]
        public void TestHUProduct01()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>1960</opfoerelsesaar><bebygget_m2>20</bebygget_m2><bolig_m2>180</bolig_m2><garage_m2>10</garage_m2><bo_type>110</bo_type><etager>2</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>true</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>false</svampinsekt><raad>false</raad><roerkablerstikledninger>false</roerkablerstikledninger><selvrisiko>150</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; 
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        [TestMethod]
        public void TestHUProduct02()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>1980</opfoerelsesaar><bebygget_m2>20</bebygget_m2><bolig_m2>180</bolig_m2><garage_m2>10</garage_m2><bo_type>110</bo_type><etager>2</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>true</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>true</svampinsekt><raad>false</raad><roerkablerstikledninger>false</roerkablerstikledninger><selvrisiko>100</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; 
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        [TestMethod]
        public void TestHUProduct03()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>200</opfoerelsesaar><bebygget_m2>20</bebygget_m2><bolig_m2>180</bolig_m2><garage_m2>10</garage_m2><bo_type>110</bo_type><etager>2</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>true</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>false</svampinsekt><raad>false</raad><roerkablerstikledninger>true</roerkablerstikledninger><selvrisiko>200</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        [TestMethod]
        public void TestHUProduct04()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>2014</opfoerelsesaar><bebygget_m2>20</bebygget_m2><bolig_m2>180</bolig_m2><garage_m2>10</garage_m2><bo_type>110</bo_type><etager>2</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>true</hoejvandslukke><nedlagt_landbrug>true</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>false</svampinsekt><raad>false</raad><roerkablerstikledninger>false</roerkablerstikledninger><selvrisiko>500</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        [TestMethod]
        public void TestHUProduct05()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>1960</opfoerelsesaar><bebygget_m2>150</bebygget_m2><bolig_m2>10</bolig_m2><garage_m2>10</garage_m2><bo_type>110</bo_type><etager>1</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>true</hoejvandslukke><nedlagt_landbrug>true</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>true</svampinsekt><raad>false</raad><roerkablerstikledninger>false</roerkablerstikledninger><selvrisiko>500</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        [TestMethod]
        public void TestHUProduct06()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>1980</opfoerelsesaar><bebygget_m2>150</bebygget_m2><bolig_m2>10</bolig_m2><garage_m2>10</garage_m2><bo_type>110</bo_type><etager>1</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>true</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>false</svampinsekt><raad>false</raad><roerkablerstikledninger>true</roerkablerstikledninger><selvrisiko>200</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        [TestMethod]
        public void TestHUProduct07()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>200</opfoerelsesaar><bebygget_m2>150</bebygget_m2><bolig_m2>10</bolig_m2><garage_m2>10</garage_m2><bo_type>110</bo_type><etager>1</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>true</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>false</svampinsekt><raad>false</raad><roerkablerstikledninger>false</roerkablerstikledninger><selvrisiko>100</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        [TestMethod]
        public void TestHUProduct08()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>2014</opfoerelsesaar><bebygget_m2>150</bebygget_m2><bolig_m2>10</bolig_m2><garage_m2>10</garage_m2><bo_type>110</bo_type><etager>1</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>true</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>true</svampinsekt><raad>false</raad><roerkablerstikledninger>false</roerkablerstikledninger><selvrisiko>0</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        [TestMethod]
        public void TestHUProduct09()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>1960</opfoerelsesaar><bebygget_m2>20</bebygget_m2><bolig_m2>180</bolig_m2><garage_m2>10</garage_m2><bo_type>110</bo_type><etager>1</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>true</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>false</svampinsekt><raad>true</raad><roerkablerstikledninger>true</roerkablerstikledninger><selvrisiko>500</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        [TestMethod]
        public void TestHUProduct010()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>1980</opfoerelsesaar><bebygget_m2>20</bebygget_m2><bolig_m2>180</bolig_m2><garage_m2>10</garage_m2><bo_type>110</bo_type><etager>1</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>true</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>false</svampinsekt><raad>false</raad><roerkablerstikledninger>false</roerkablerstikledninger><selvrisiko>200</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        [TestMethod]
        public void TestHUProduct011()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>200</opfoerelsesaar><bebygget_m2>20</bebygget_m2><bolig_m2>180</bolig_m2><garage_m2>10</garage_m2><bo_type>110</bo_type><etager>1</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>true</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>true</svampinsekt><raad>true</raad><roerkablerstikledninger>false</roerkablerstikledninger><selvrisiko>100</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        [TestMethod]
        public void TestHUProduct012()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>2014</opfoerelsesaar><bebygget_m2>20</bebygget_m2><bolig_m2>180</bolig_m2><garage_m2>10</garage_m2><bo_type>110</bo_type><etager>1</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>true</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>false</svampinsekt><raad>false</raad><roerkablerstikledninger>true</roerkablerstikledninger><selvrisiko>0</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        [TestMethod]
        public void TestHUProduct013()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>1960</opfoerelsesaar><bebygget_m2>150</bebygget_m2><bolig_m2>10</bolig_m2><garage_m2>0</garage_m2><bo_type>110</bo_type><etager>2</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>false</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>false</svampinsekt><raad>true</raad><roerkablerstikledninger>false</roerkablerstikledninger><selvrisiko>0</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; 
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        [TestMethod]
        public void TestHUProduct014()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>1980</opfoerelsesaar><bebygget_m2>150</bebygget_m2><bolig_m2>10</bolig_m2><garage_m2>0</garage_m2><bo_type>110</bo_type><etager>2</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>false</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>true</svampinsekt><raad>false</raad><roerkablerstikledninger>false</roerkablerstikledninger><selvrisiko>100</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        [TestMethod]
        public void TestHUProduct015()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>200</opfoerelsesaar><bebygget_m2>150</bebygget_m2><bolig_m2>10</bolig_m2><garage_m2>0</garage_m2><bo_type>110</bo_type><etager>2</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>false</hoejvandslukke><nedlagt_landbrug>true</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>false</svampinsekt><raad>true</raad><roerkablerstikledninger>true</roerkablerstikledninger><selvrisiko>200</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        [TestMethod]
        public void TestHUProduct016()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.9\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>fb726c66-4a12-42c8-ae34-7df152938d6f</sessionid><insurancerequest><hus><opfoerelsesaar>2014</opfoerelsesaar><bebygget_m2>150</bebygget_m2><bolig_m2>10</bolig_m2><garage_m2>0</garage_m2><bo_type>110</bo_type><etager>2</etager><tagbeklaedning>1</tagbeklaedning><kaelder_m2>50</kaelder_m2><antal_skader_treaar>1</antal_skader_treaar><opvarmning>1</opvarmning><pool_placering>udendoers</pool_placering><pool_m2>0</pool_m2><vandstop>true</vandstop><hoejvandslukke>false</hoejvandslukke><nedlagt_landbrug>true</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><vaadrum>2</vaadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>false</svampinsekt><raad>false</raad><roerkablerstikledninger>false</roerkablerstikledninger><selvrisiko>500</selvrisiko><rabatordningvalg /></hus></insurancerequest></request>"; string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestHUProduct017()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, Tarup, 5210 Odense NV</adresse><kvhx>46135020019000000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>c5f4ba93-c033-4e23-b186-d856c14953d4</sessionid><insurancerequest><hus><opfoerelsesaar>1971</opfoerelsesaar><bebygget_m2>127</bebygget_m2><bolig_m2>127</bolig_m2><garage_m2>0</garage_m2><etager>1</etager><tagbeklaedning>5</tagbeklaedning><kaelder_m2>0</kaelder_m2><antal_skader_treaar>0</antal_skader_treaar><opvarmning>1</opvarmning><pool_m2>0</pool_m2><pool_placering>indendoers</pool_placering><vandstop>false</vandstop><hoejvandslukke>false</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>true</svampinsekt><raad>false</raad><roerkablerstikledninger>false</roerkablerstikledninger><selvrisiko>2000</selvrisiko><bo_type>120</bo_type><ydervaeg_kode>2</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>1</varmeinstallation><erhverv_m2>0</erhverv_m2><lejligheder>1</lejligheder><overdaek_m2>0</overdaek_m2><tagetage_lovlig_m2>0</tagetage_lovlig_m2></hus></insurancerequest></request>"; 
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestHUProduct018()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, Tarup, 5210 Odense NV</adresse><kvhx>46135020019000000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>c5f4ba93-c033-4e23-b186-d856c14953d4</sessionid><insurancerequest><hus><opfoerelsesaar>1971</opfoerelsesaar><bebygget_m2>127</bebygget_m2><bolig_m2>127</bolig_m2><garage_m2>0</garage_m2><etager>1</etager><tagbeklaedning>5</tagbeklaedning><kaelder_m2>0</kaelder_m2><antal_skader_treaar>0</antal_skader_treaar><opvarmning>1</opvarmning><pool_m2>0</pool_m2><pool_placering>indendoers</pool_placering><vandstop>false</vandstop><hoejvandslukke>false</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>true</dyrskader><kosmetiskdaekning>true</kosmetiskdaekning><svampinsekt>true</svampinsekt><raad>true</raad><roerkablerstikledninger>true</roerkablerstikledninger><selvrisiko>2000</selvrisiko><bo_type>120</bo_type><ydervaeg_kode>2</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>1</varmeinstallation><erhverv_m2>0</erhverv_m2><lejligheder>1</lejligheder><overdaek_m2>0</overdaek_m2><tagetage_lovlig_m2>0</tagetage_lovlig_m2></hus></insurancerequest></request>"; 
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestHUProduct19()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, Tarup, 5210 Odense NV</adresse><kvhx>46135020019000000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>c5f4ba93-c033-4e23-b186-d856c14953d4</sessionid><insurancerequest><hus><opfoerelsesaar>1971</opfoerelsesaar><bebygget_m2>127</bebygget_m2><bolig_m2>127</bolig_m2><garage_m2>0</garage_m2><etager>1</etager><tagbeklaedning>5</tagbeklaedning><kaelder_m2>0</kaelder_m2><antal_skader_treaar>0</antal_skader_treaar><opvarmning>1</opvarmning><pool_m2>0</pool_m2><pool_placering>indendoers</pool_placering><vandstop>false</vandstop><hoejvandslukke>false</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>2</toiletbadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>true</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>true</svampinsekt><raad>true</raad><roerkablerstikledninger>true</roerkablerstikledninger><selvrisiko>25000</selvrisiko><bo_type>120</bo_type><ydervaeg_kode>2</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>1</varmeinstallation><erhverv_m2>0</erhverv_m2><lejligheder>1</lejligheder><overdaek_m2>0</overdaek_m2><tagetage_lovlig_m2>0</tagetage_lovlig_m2></hus></insurancerequest></request>"; 
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        
        [TestMethod]
        public void TestBAProduct20()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, Tarup, 5210 Odense NV</adresse><kvhx>46135020019000000</kvhx></customer><insurancerequestsforcalculation>indbo</insurancerequestsforcalculation><sessionid>fe9e66c2-b2ee-4657-b8ad-49b213056604</sessionid><insurancerequest><indbo><antal_voksne>2</antal_voksne><antal_boern>2</antal_boern><bolig_m2>127</bolig_m2><antal_skader_treaar>0</antal_skader_treaar><kaelder_m2>0</kaelder_m2><tagbeklaedning>5</tagbeklaedning><straatag_brandisoleret>false</straatag_brandisoleret><eksisterende_forsikring>true</eksisterende_forsikring><alarm>false</alarm><tilknyttet_alarmcentral>false</tilknyttet_alarmcentral><vindueslaas>false</vindueslaas><roegalarm>true</roegalarm><glaskumme>false</glaskumme><pludseligskade>false</pludseligskade><elektronik>false</elektronik><rejseverden>false</rejseverden><rejseeuropa>false</rejseeuropa><afbestillingsforsikring>false</afbestillingsforsikring><indbosum>10000000</indbosum><selvrisiko>2000</selvrisiko><bo_type>120</bo_type><ydervaeg_kode>90</ydervaeg_kode><erhverv_m2>0</erhverv_m2><lejligheder>1</lejligheder></indbo></insurancerequest></request>"; 
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }        

        [TestMethod]
        public void TestULProduct01()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.8\"><customer><alder>41</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>ulykke</insurancerequestsforcalculation><sessionid>71838cda-5334-4910-8a6b-9057f76c028</sessionid><insurancerequest><ulykke><stillingsbetegnelse>Pensionist</stillingsbetegnelse><stillingsbetegnelseId>1160</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>100000</forsikringssum_varigt_men><forsikringssum_doed>1000000</forsikringssum_doed><dobbelterstatning>false</dobbelterstatning><farligsport>true</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>true</strakserstatning><tilvalg5>false</tilvalg5><tilvalg6>true</tilvalg6><samlever><alder>40</alder><stillingsbetegnelse>Pensionist</stillingsbetegnelse><stillingsbetegnelseId>1160</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>100000</forsikringssum_varigt_men><forsikringssum_doed>1000000</forsikringssum_doed><dobbelterstatning>false</dobbelterstatning><farligsport>true</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>true</strakserstatning><tilvalg5>false</tilvalg5><tilvalg6>true</tilvalg6></samlever><boern><antal_u18>2</antal_u18><forsikringssum_varigt_men>100000</forsikringssum_varigt_men><dobbelterstatning>true</dobbelterstatning><farligsport>true</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>true</strakserstatning><boerneulykke>false</boerneulykke><tilvalg6>true</tilvalg6></boern><rabatordningvalg /></ulykke></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestULProduct02()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>42</alder><postnr>0</postnr></customer><insurancerequestsforcalculation>ulykke</insurancerequestsforcalculation><sessionid>c8b2f36f-baa8-40ee-8547-423b390d4b94</sessionid><insurancerequest><ulykke><stillingsbetegnelse>Arkitekt, udelukkende kontor</stillingsbetegnelse><stillingsbetegnelseId>55</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><forsikringssum_doed>100000</forsikringssum_doed><dobbelterstatning>false</dobbelterstatning><farligsport>false</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning></ulykke></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestULProduct03()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.8\"><customer><alder>41</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>ulykke</insurancerequestsforcalculation><sessionid>71838cda-5334-4910-8a6b-9057f76c028</sessionid><insurancerequest><ulykke><stillingsbetegnelse>Revisor</stillingsbetegnelse><stillingsbetegnelseId>93</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>1000</forsikringssum_varigt_men><forsikringssum_doed>1000</forsikringssum_doed><dobbelterstatning>false</dobbelterstatning><farligsport>true</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning><tilvalg5>false</tilvalg5><tilvalg6>false</tilvalg6><samlever><alder>40</alder><stillingsbetegnelse>Psykolog</stillingsbetegnelse><stillingsbetegnelseId>27</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>1000</forsikringssum_varigt_men><forsikringssum_doed>1000</forsikringssum_doed><dobbelterstatning>false</dobbelterstatning><farligsport>true</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning><tilvalg5>false</tilvalg5><tilvalg6>false</tilvalg6></samlever><boern><antal_u18>1</antal_u18><forsikringssum_varigt_men>1000</forsikringssum_varigt_men><dobbelterstatning>false</dobbelterstatning><farligsport>false</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning><boerneulykke>false</boerneulykke><tilvalg6>false</tilvalg6></boern><rabatordningvalg /></ulykke></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestULProduct04()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>42</alder><postnr>0</postnr><kvhx>00000000000000000</kvhx></customer><insurancerequestsforcalculation>ulykke</insurancerequestsforcalculation><sessionid>cc98526d-0798-4bc2-aa62-6c86dc0bec49</sessionid><insurancerequest><ulykke><stillingsbetegnelse>Systemudvikler</stillingsbetegnelse><stillingsbetegnelseId>1500</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><forsikringssum_doed>100000</forsikringssum_doed><dobbelterstatning>false</dobbelterstatning><farligsport>false</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning><samlever><alder>38</alder><stillingsbetegnelse>IT supporter</stillingsbetegnelse><stillingsbetegnelseId>673</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><forsikringssum_doed>100000</forsikringssum_doed><dobbelterstatning>false</dobbelterstatning><farligsport>false</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning></samlever><boern><antal_u18>2</antal_u18><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><dobbelterstatning>false</dobbelterstatning><farligsport>false</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning></boern></ulykke></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestULProduct05()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>42</alder><postnr>0</postnr></customer><insurancerequestsforcalculation>ulykke</insurancerequestsforcalculation><sessionid>0b61e05b-9e90-4131-a712-acf00b844ac1</sessionid><insurancerequest><ulykke><stillingsbetegnelse>Systemudvikler</stillingsbetegnelse><stillingsbetegnelseId>1500</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><forsikringssum_doed>100000</forsikringssum_doed><dobbelterstatning>true</dobbelterstatning><farligsport>true</farligsport><motorcykelknallert>true</motorcykelknallert><strakserstatning>false</strakserstatning><samlever><alder>0</alder><stillingsbetegnelseId>0</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><forsikringssum_doed>100000</forsikringssum_doed><dobbelterstatning>false</dobbelterstatning><farligsport>true</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning></samlever><boern><antal_u18>0</antal_u18><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><dobbelterstatning>false</dobbelterstatning><farligsport>false</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning></boern></ulykke></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestULProduct06()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.15\"><customer><alder>42</alder><postnr xsi:nil=\"true\" /></customer><insurancerequestsforcalculation>ulykke</insurancerequestsforcalculation><sessionid>0b61e05b-9e90-4131-a712-acf00b844ac1</sessionid><insurancerequest><ulykke><stillingsbetegnelse>Systemudvikler</stillingsbetegnelse><stillingsbetegnelseId>1500</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><forsikringssum_doed>100000</forsikringssum_doed><dobbelterstatning>false</dobbelterstatning><farligsport>false</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning><samlever><alder>0</alder><stillingsbetegnelseId>0</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><forsikringssum_doed>100000</forsikringssum_doed><dobbelterstatning>false</dobbelterstatning><farligsport>false</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning></samlever><boern><antal_u18>0</antal_u18><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><dobbelterstatning>false</dobbelterstatning><farligsport>false</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning></boern></ulykke></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestULProduct07()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.15\"><customer><alder>42</alder><postnr xsi:nil=\"true\" /></customer><insurancerequestsforcalculation>ulykke</insurancerequestsforcalculation><sessionid>0b61e05b-9e90-4131-a712-acf00b844ac1</sessionid><insurancerequest><ulykke><stillingsbetegnelse>Systemudvikler</stillingsbetegnelse><stillingsbetegnelseId>1500</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><forsikringssum_doed>100000</forsikringssum_doed><dobbelterstatning>false</dobbelterstatning><farligsport>true</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning><samlever><alder>38</alder><stillingsbetegnelse>IT supporter</stillingsbetegnelse><stillingsbetegnelseId>673</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><forsikringssum_doed>100000</forsikringssum_doed><dobbelterstatning>false</dobbelterstatning><farligsport>true</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning></samlever><boern><antal_u18>1</antal_u18><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><dobbelterstatning>false</dobbelterstatning><farligsport>false</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning></boern></ulykke></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestULProduct08()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>44</alder><postnr>0</postnr></customer><insurancerequestsforcalculation>ulykke</insurancerequestsforcalculation><sessionid>94ebc350-c100-4c30-b8e3-fd03ec711585</sessionid><insurancerequest><ulykke><stillingsbetegnelse>Arkitekt - med tilsyn</stillingsbetegnelse><stillingsbetegnelseId>54</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><forsikringssum_doed>100000</forsikringssum_doed><dobbelterstatning>false</dobbelterstatning><farligsport>false</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning><samlever><alder>66</alder><stillingsbetegnelse>Arkitekt - med tilsyn</stillingsbetegnelse><stillingsbetegnelseId>54</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><forsikringssum_doed>100000</forsikringssum_doed><dobbelterstatning>false</dobbelterstatning><farligsport>false</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning></samlever></ulykke></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
        


        //<request xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" schemaVersion="1.14"><customer><alder>42</alder><postnr>0</postnr></customer><insurancerequestsforcalculation>ulykke</insurancerequestsforcalculation><sessionid>102aaa85-70f2-4fcb-9ee7-913cc243a669</sessionid><insurancerequest><ulykke><stillingsbetegnelse>Systemudvikler</stillingsbetegnelse><stillingsbetegnelseId>1500</stillingsbetegnelseId><heltidsulykke>true</heltidsulykke><forsikringssum_varigt_men>1000000</forsikringssum_varigt_men><forsikringssum_doed>100000</forsikringssum_doed><dobbelterstatning>false</dobbelterstatning><farligsport>false</farligsport><motorcykelknallert>false</motorcykelknallert><strakserstatning>false</strakserstatning></ulykke></insurancerequest></request>
        
        

        /*
         * TestBIProduct01
            customer	
            alder	42
            postnr	5210
            adresse	Hvidkløvervej 19, 5210 Odense NV
            kvhx	2652104005000
	
            insurancerequestsforcalculation	bil
            sessionid	fe2b0541-cd69-4076-b540-4c4a9958c028
            insurancerequest	
            bil	
            koersel_aar	1500
            model	Mondeo
            modelaar	208
            motor	2,0
            fabrikat	Ford
            fabrikatkode	55
            modelkode	550
            nypris	6500
            hk	147
            modelvariantid	24836207
            variant	
            typenavn	P
            egenvaegt	1047
            koereklarvaegt	1158
            dktypegodkendelsesnumre	14692-05; 53842-05; 51784-04
            drivmiddel	Benzin
            kw	64
            stelnummer	WF0EXXGBBE8L32220
            foerste_reg	208
            reg_nr	GG35577
            kid	10830120818731
            ansvar_kasko	true
            selvrisiko	1605
            friskade	true
            brugere_u25	false
            udvidetglas	true
            fastpraemie	false
            foererdaekning	true
            privateleaset	false
            vejhjaelp	false
            antalaarmedbilforsikring	10
            antalskader	0
            skade1	0
            skade2	0
            skade3	0
            klipikoerekort	false
            rabatordningvalg 	false
         */
                



        /*[TestMethod]
        public void testClub()
        {
            string error;
            ReferenceServiceClient.TIAReferenceServiceClient client = new TIAReferenceServiceClient();
            gf_club_request gfc_req = new gf_club_request();
            gf_club_response gfc_res = new gf_club_response();
            gfc_req.company_profession_code = "5";
            gfc_req.zip_code = "5210";
            client.GetClubNumber(gfc_req, out gfc_res, out error);
           
        }*/

        private void writeXML(string XML, string testName)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\temp\\" + testName + ".xml");
            file.WriteLine(XML);
            file.Close();
        }



    }
}