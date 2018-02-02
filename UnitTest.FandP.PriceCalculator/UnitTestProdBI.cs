using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient;
//using UnitTest.FandP.PriceCalc.ReferenceServiceClient;

using System.Xml;

namespace UnitTest.FandP.PriceCalculator
{
    [TestClass]
    public class UnitTestProdBI
    {
        [TestMethod]
        public void TestBIProduct01()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.8\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>fe2b0541-cd69-4076-b540-4c4a9958c028</sessionid><insurancerequest><bil><koersel_aar>1500</koersel_aar><model>Mondeo</model><modelaar>208</modelaar><motor>2,0</motor><fabrikat>Ford</fabrikat><fabrikatkode>55</fabrikatkode><modelkode>550</modelkode><nypris>6500</nypris><hk>147</hk><modelvariantid>24836207</modelvariantid><variant></variant><typenavn>P</typenavn><egenvaegt>1047</egenvaegt><koereklarvaegt>1158</koereklarvaegt><dktypegodkendelsesnumre>14692-05; 53842-05; 51784-04</dktypegodkendelsesnumre><drivmiddel>Benzin</drivmiddel><kw>64</kw><stelnummer>WF0EXXGBBE8L32220</stelnummer><foerste_reg>2008</foerste_reg><reg_nr>GG35577</reg_nr><kid>10830120818731</kid><ansvar_kasko>true</ansvar_kasko><selvrisiko>1605</selvrisiko><friskade>true</friskade><brugere_u25>false</brugere_u25><udvidetglas>true</udvidetglas><fastpraemie>false</fastpraemie><foererdaekning>true</foererdaekning><privateleaset>false</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>10</antalaarmedbilforsikring><antalskader>0</antalskader><skade1>0</skade1><skade2>0</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort><rabatordningvalg>false</rabatordningvalg></bil></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            writeXML(xmlResponse, "TestBIProduct01");
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestBIProduct02()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.8\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>fe2b0541-cd69-4076-b540-4c4a9958c028</sessionid><insurancerequest><bil><koersel_aar>1500</koersel_aar><model>Mondeo</model><modelaar>208</modelaar><motor>2,0</motor><fabrikat>Ford</fabrikat><fabrikatkode>55</fabrikatkode><modelkode>550</modelkode><nypris>6500</nypris><hk>147</hk><modelvariantid>24836207</modelvariantid><variant></variant><typenavn>P</typenavn><egenvaegt>1047</egenvaegt><koereklarvaegt>1158</koereklarvaegt><dktypegodkendelsesnumre>14692-05; 53842-05; 51784-04</dktypegodkendelsesnumre><drivmiddel>Benzin</drivmiddel><kw>64</kw><stelnummer>WF0EXXGBBE8L32220</stelnummer><foerste_reg>2008</foerste_reg><reg_nr>GG35577</reg_nr><kid>10830120818731</kid><ansvar_kasko>true</ansvar_kasko><selvrisiko>200</selvrisiko><friskade>true</friskade><brugere_u25>false</brugere_u25><udvidetglas>true</udvidetglas><fastpraemie>false</fastpraemie><foererdaekning>true</foererdaekning><privateleaset>false</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>10</antalaarmedbilforsikring><antalskader>0</antalskader><skade1>0</skade1><skade2>0</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort><rabatordningvalg >false</rabatordningvalg ></bil></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            writeXML(xmlResponse, "TestBIProduct02");
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestBIProduct03()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.8\"><customer><alder>40</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>fe2b0541-cd69-4076-b540-4c4a9958c028</sessionid><insurancerequest><bil><koersel_aar>200</koersel_aar><model>Passat</model><modelaar>2010</modelaar><motor>1,4</motor><fabrikat>VW</fabrikat><fabrikatkode>55</fabrikatkode><modelkode>550</modelkode><nypris>172378</nypris><hk>360</hk><modelvariantid>24836207</modelvariantid><variant>1,4 D-4D Linea Terra 90HK 3d (3 D, 90hk)</variant><typenavn>P</typenavn><egenvaegt>1047</egenvaegt><koereklarvaegt>1158</koereklarvaegt><dktypegodkendelsesnumre>14692-05; 53842-05; 51784-04</dktypegodkendelsesnumre><drivmiddel>Benzin</drivmiddel><kw>64</kw><stelnummer>VNKKC96330A105414</stelnummer><foerste_reg>2007-06-03</foerste_reg><reg_nr>AB61915</reg_nr><kid>102650120715732</kid><ansvar_kasko>true</ansvar_kasko><selvrisiko>600</selvrisiko><friskade>false</friskade><brugere_u25>false</brugere_u25><udvidetglas>true</udvidetglas><fastpraemie>true</fastpraemie><foererdaekning>false</foererdaekning><privateleaset>true</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>10</antalaarmedbilforsikring><antalskader>2</antalskader><skade1>2013</skade1><skade2>2011</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort><rabatordningvalg /></bil></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            writeXML(xmlResponse, "TestBIProduct03");
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestBIProduct04()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.8\"><customer><alder>65</alder><postnr>2610</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>fe2b0541-cd69-4076-b540-4c4a9958c028</sessionid><insurancerequest><bil><koersel_aar>100</koersel_aar><model>Passat</model><modelaar>2010</modelaar><motor>1,4</motor><fabrikat>VW</fabrikat><fabrikatkode>55</fabrikatkode><modelkode>550</modelkode><nypris>172378</nypris><hk>90</hk><modelvariantid>24836207</modelvariantid><variant>1,4 D-4D Linea Terra 90HK 3d (3 D, 90hk)</variant><typenavn>P</typenavn><egenvaegt>1047</egenvaegt><koereklarvaegt>1158</koereklarvaegt><dktypegodkendelsesnumre>14692-05; 53842-05; 51784-04</dktypegodkendelsesnumre><drivmiddel>Diesel</drivmiddel><kw>64</kw><stelnummer>VNKKC96330A105414</stelnummer><foerste_reg>2007-06-03</foerste_reg><reg_nr>AB61915</reg_nr><kid>102650120715732</kid><ansvar_kasko>true</ansvar_kasko><selvrisiko>100</selvrisiko><friskade>false</friskade><brugere_u25>false</brugere_u25><udvidetglas>true</udvidetglas><fastpraemie>true</fastpraemie><foererdaekning>false</foererdaekning><privateleaset>true</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>10</antalaarmedbilforsikring><antalskader>3</antalskader><skade1>2013</skade1><skade2>2011</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort><rabatordningvalg /></bil></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            writeXML(xmlResponse, "TestBIProduct04");
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestBIProduct05()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.8\"><customer><alder>18</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>fe2b0541-cd69-4076-b540-4c4a9958c028</sessionid><insurancerequest><bil><koersel_aar>1500</koersel_aar><model>Passat</model><modelaar>2010</modelaar><motor>1,4</motor><fabrikat>VW</fabrikat><fabrikatkode>55</fabrikatkode><modelkode>550</modelkode><nypris>172378</nypris><hk>180</hk><modelvariantid>24836207</modelvariantid><variant>1,4 D-4D Linea Terra 90HK 3d (3 D, 90hk)</variant><typenavn>P</typenavn><egenvaegt>1047</egenvaegt><koereklarvaegt>1158</koereklarvaegt><dktypegodkendelsesnumre>14692-05; 53842-05; 51784-04</dktypegodkendelsesnumre><drivmiddel>Benzin</drivmiddel><kw>64</kw><stelnummer>VNKKC96330A105414</stelnummer><foerste_reg>2007-06-03</foerste_reg><reg_nr>AB61915</reg_nr><kid>102650120715732</kid><ansvar_kasko>true</ansvar_kasko><selvrisiko>200</selvrisiko><friskade>false</friskade><brugere_u25>false</brugere_u25><udvidetglas>true</udvidetglas><fastpraemie>true</fastpraemie><foererdaekning>false</foererdaekning><privateleaset>true</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>10</antalaarmedbilforsikring><antalskader>4</antalskader><skade1>2013</skade1><skade2>2011</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort><rabatordningvalg /></bil></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            writeXML(xmlResponse, "TestBIProduct05");
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestBIProduct06()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.8\"><customer><alder>24</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>fe2b0541-cd69-4076-b540-4c4a9958c028</sessionid><insurancerequest><bil><koersel_aar>200</koersel_aar><model>Passat</model><modelaar>2010</modelaar><motor>1,4</motor><fabrikat>VW</fabrikat><fabrikatkode>55</fabrikatkode><modelkode>550</modelkode><nypris>172378</nypris><hk>360</hk><modelvariantid>24836207</modelvariantid><variant>1,4 D-4D Linea Terra 90HK 3d (3 D, 90hk)</variant><typenavn>P</typenavn><egenvaegt>1047</egenvaegt><koereklarvaegt>1158</koereklarvaegt><dktypegodkendelsesnumre>14692-05; 53842-05; 51784-04</dktypegodkendelsesnumre><drivmiddel>Diesel</drivmiddel><kw>64</kw><stelnummer>VNKKC96330A105414</stelnummer><foerste_reg>2007-06-03</foerste_reg><reg_nr>AB61915</reg_nr><kid>102650120715732</kid><ansvar_kasko>true</ansvar_kasko><selvrisiko>400</selvrisiko><friskade>false</friskade><brugere_u25>false</brugere_u25><udvidetglas>true</udvidetglas><fastpraemie>true</fastpraemie><foererdaekning>false</foererdaekning><privateleaset>true</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>10</antalaarmedbilforsikring><antalskader>5</antalskader><skade1>2013</skade1><skade2>2011</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort><rabatordningvalg /></bil></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            writeXML(xmlResponse, "TestBIProduct06");
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void TestBIProduct07()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.8\"><customer><alder>40</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>fe2b0541-cd69-4076-b540-4c4a9958c028</sessionid><insurancerequest><bil><koersel_aar>100</koersel_aar><model>Passat</model><modelaar>2010</modelaar><motor>1,4</motor><fabrikat>VW</fabrikat><fabrikatkode>55</fabrikatkode><modelkode>550</modelkode><nypris>172378</nypris><hk>90</hk><modelvariantid>24836207</modelvariantid><variant>1,4 D-4D Linea Terra 90HK 3d (3 D, 90hk)</variant><typenavn>P</typenavn><egenvaegt>1047</egenvaegt><koereklarvaegt>1158</koereklarvaegt><dktypegodkendelsesnumre>14692-05; 53842-05; 51784-04</dktypegodkendelsesnumre><drivmiddel>Benzin</drivmiddel><kw>64</kw><stelnummer>VNKKC96330A105414</stelnummer><foerste_reg>2007-06-03</foerste_reg><reg_nr>AB61915</reg_nr><kid>102650120715732</kid><ansvar_kasko>false</ansvar_kasko><selvrisiko>600</selvrisiko><friskade>false</friskade><brugere_u25>false</brugere_u25><udvidetglas>true</udvidetglas><fastpraemie>true</fastpraemie><foererdaekning>false</foererdaekning><privateleaset>true</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>10</antalaarmedbilforsikring><antalskader>6</antalskader><skade1>2013</skade1><skade2>2011</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort><rabatordningvalg /></bil></insurancerequest></request>";
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
        public void TestBIProduct08()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.8\"><customer><alder>65</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>fe2b0541-cd69-4076-b540-4c4a9958c028</sessionid><insurancerequest><bil><koersel_aar>1500</koersel_aar><model>Passat</model><modelaar>2010</modelaar><motor>1,4</motor><fabrikat>VW</fabrikat><fabrikatkode>55</fabrikatkode><modelkode>550</modelkode><nypris>172378</nypris><hk>180</hk><modelvariantid>24836207</modelvariantid><variant>1,4 D-4D Linea Terra 90HK 3d (3 D, 90hk)</variant><typenavn>P</typenavn><egenvaegt>1047</egenvaegt><koereklarvaegt>1158</koereklarvaegt><dktypegodkendelsesnumre>14692-05; 53842-05; 51784-04</dktypegodkendelsesnumre><drivmiddel>Diesel</drivmiddel><kw>64</kw><stelnummer>VNKKC96330A105414</stelnummer><foerste_reg>2007-06-03</foerste_reg><reg_nr>AB61915</reg_nr><kid>102650120715732</kid><ansvar_kasko>false</ansvar_kasko><selvrisiko>100</selvrisiko><friskade>false</friskade><brugere_u25>false</brugere_u25><udvidetglas>true</udvidetglas><fastpraemie>true</fastpraemie><foererdaekning>false</foererdaekning><privateleaset>true</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>10</antalaarmedbilforsikring><antalskader>1</antalskader><skade1>2013</skade1><skade2>2011</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort><rabatordningvalg /></bil></insurancerequest></request>";
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
        public void TestBIProduct09()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.8\"><customer><alder>18</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>fe2b0541-cd69-4076-b540-4c4a9958c028</sessionid><insurancerequest><bil><koersel_aar>200</koersel_aar><model>Passat</model><modelaar>2010</modelaar><motor>1,4</motor><fabrikat>VW</fabrikat><fabrikatkode>55</fabrikatkode><modelkode>550</modelkode><nypris>172378</nypris><hk>360</hk><modelvariantid>24836207</modelvariantid><variant>1,4 D-4D Linea Terra 90HK 3d (3 D, 90hk)</variant><typenavn>P</typenavn><egenvaegt>1047</egenvaegt><koereklarvaegt>1158</koereklarvaegt><dktypegodkendelsesnumre>14692-05; 53842-05; 51784-04</dktypegodkendelsesnumre><drivmiddel>Benzin</drivmiddel><kw>64</kw><stelnummer>VNKKC96330A105414</stelnummer><foerste_reg>2007-06-03</foerste_reg><reg_nr>AB61915</reg_nr><kid>102650120715732</kid><ansvar_kasko>false</ansvar_kasko><selvrisiko>200</selvrisiko><friskade>false</friskade><brugere_u25>false</brugere_u25><udvidetglas>true</udvidetglas><fastpraemie>true</fastpraemie><foererdaekning>false</foererdaekning><privateleaset>true</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>10</antalaarmedbilforsikring><antalskader>2</antalskader><skade1>2013</skade1><skade2>2011</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort><rabatordningvalg /></bil></insurancerequest></request>";
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
        public void TestBIProduct010()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.8\"><customer><alder>40</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>71838cda-5334-4910-8a6b-9057f76c028</sessionid><insurancerequest><bil><koersel_aar>1500</koersel_aar><model>Passat</model><modelaar>2010</modelaar><motor>1,4</motor><fabrikat>VW</fabrikat><fabrikatkode>55</fabrikatkode><modelkode>550</modelkode><nypris>172378</nypris><hk>90</hk><modelvariantid>24836207</modelvariantid><variant>1,4 D-4D Linea Terra 90HK 3d (3 D, 90hk)</variant><typenavn>P</typenavn><egenvaegt>1047</egenvaegt><koereklarvaegt>1158</koereklarvaegt><dktypegodkendelsesnumre>14692-05; 53842-05; 51784-04</dktypegodkendelsesnumre><drivmiddel>Diesel</drivmiddel><kw>64</kw><stelnummer>VNKKC96330A105414</stelnummer><foerste_reg>2007-06-03</foerste_reg><reg_nr>AB61915</reg_nr><kid>102650120715732</kid><ansvar_kasko>false</ansvar_kasko><selvrisiko>500</selvrisiko><friskade>false</friskade><brugere_u25>false</brugere_u25><udvidetglas>true</udvidetglas><fastpraemie>true</fastpraemie><foererdaekning>false</foererdaekning><privateleaset>true</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>10</antalaarmedbilforsikring><antalskader>2</antalskader><skade1>2013</skade1><skade2>2011</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort><rabatordningvalg /></bil></insurancerequest></request>";
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
        public void TestBIProduct011()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.8\"><customer><alder>40</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>fe2b0541-cd69-4076-b540-4c4a9958c028</sessionid><insurancerequest><bil><koersel_aar>1500</koersel_aar><model>Passat</model><modelaar>2010</modelaar><motor>1,4</motor><fabrikat>VW</fabrikat><fabrikatkode>55</fabrikatkode><modelkode>550</modelkode><nypris>172378</nypris><hk>180</hk><modelvariantid>24836207</modelvariantid><variant>1,4 D-4D Linea Terra 90HK 3d (3 D, 90hk)</variant><typenavn>P</typenavn><egenvaegt>1047</egenvaegt><koereklarvaegt>1158</koereklarvaegt><dktypegodkendelsesnumre>14692-05; 53842-05; 51784-04</dktypegodkendelsesnumre><drivmiddel>Benzin</drivmiddel><kw>64</kw><stelnummer>VNKKC96330A105414</stelnummer><foerste_reg>2007-06-03</foerste_reg><reg_nr>AB61915</reg_nr><kid>102650120715732</kid><ansvar_kasko>false</ansvar_kasko><selvrisiko>600</selvrisiko><friskade>false</friskade><brugere_u25>false</brugere_u25><udvidetglas>true</udvidetglas><fastpraemie>true</fastpraemie><foererdaekning>false</foererdaekning><privateleaset>true</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>10</antalaarmedbilforsikring><antalskader>4</antalskader><skade1>2013</skade1><skade2>2011</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort><rabatordningvalg /></bil></insurancerequest></request>";
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
        public void TestBIProduct012()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/201/XMLSchema\" xmlns:xsi=\"http://www.w3.org/201/XMLSchema-instance\" schemaVersion=\"1.8\"><customer><alder>65</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>2652104005000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>fe2b0541-cd69-4076-b540-4c4a9958c028</sessionid><insurancerequest><bil><koersel_aar>200</koersel_aar><model>Passat</model><modelaar>2010</modelaar><motor>1,4</motor><fabrikat>VW</fabrikat><fabrikatkode>55</fabrikatkode><modelkode>550</modelkode><nypris>172378</nypris><hk>360</hk><modelvariantid>24836207</modelvariantid><variant>1,4 D-4D Linea Terra 90HK 3d (3 D, 90hk)</variant><typenavn>P</typenavn><egenvaegt>1047</egenvaegt><koereklarvaegt>1158</koereklarvaegt><dktypegodkendelsesnumre>14692-05; 53842-05; 51784-04</dktypegodkendelsesnumre><drivmiddel>Diesel</drivmiddel><kw>64</kw><stelnummer>VNKKC96330A105414</stelnummer><foerste_reg>2007-06-03</foerste_reg><reg_nr>AB61915</reg_nr><kid>102650120715732</kid><ansvar_kasko>false</ansvar_kasko><selvrisiko>100</selvrisiko><friskade>false</friskade><brugere_u25>false</brugere_u25><udvidetglas>true</udvidetglas><fastpraemie>true</fastpraemie><foererdaekning>false</foererdaekning><privateleaset>true</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>10</antalaarmedbilforsikring><antalskader>5</antalskader><skade1>2013</skade1><skade2>2011</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort><rabatordningvalg /></bil></insurancerequest></request>";
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
        public void TestBIProduct13()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, Tarup, 5210 Odense NV</adresse><kvhx>46135020019000000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>56f09503-39c4-4316-b148-ac281ac3125d</sessionid><insurancerequest><bil><koersel_aar>15000</koersel_aar><model>Mondeo</model><modelaar>2008</modelaar><motor>2,0</motor><fabrikat>Ford</fabrikat><fabrikatkode>18</fabrikatkode><modelkode>168</modelkode><nypris>357326</nypris><hk>145</hk><modelvariantid>278682008</modelvariantid><variant>2,0 Ghia 145HK 5d</variant><typenavn>WF0EXXGBBE8L32220</typenavn><egenvaegt>1350</egenvaegt><koereklarvaegt>1477</koereklarvaegt><dktypegodkendelsesnumre>54071-01; 53358-01; 52631-02; 50332-03</dktypegodkendelsesnumre><drivmiddel>Benzin</drivmiddel><kw>107</kw><stelnummer>WF0EXXGBBE8L32220</stelnummer><foerste_reg>2008-09-30</foerste_reg><reg_nr>GG35577</reg_nr><kid>1008301200818731</kid><ansvar_kasko>true</ansvar_kasko><selvrisiko>5000</selvrisiko><friskade>false</friskade><brugere_u25>false</brugere_u25><udvidetglas>false</udvidetglas><fastpraemie>false</fastpraemie><foererdaekning>false</foererdaekning><privateleaset>false</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>14</antalaarmedbilforsikring><antalskader>0</antalskader><skade1>0</skade1><skade2>0</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort></bil></insurancerequest></request>";
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
        public void TestBIProduct14Heidi()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>44</alder><postnr>5400</postnr><adresse>Odensevej 71, 5400 Bogense</adresse><kvhx>48009110071000000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>b18237ac-5dbd-4409-bb43-783e8b65046e</sessionid><insurancerequest><hus><opfoerelsesaar>1914</opfoerelsesaar><bebygget_m2>100</bebygget_m2><bolig_m2>100</bolig_m2><garage_m2>25</garage_m2><etager>1</etager><tagbeklaedning>3</tagbeklaedning><kaelder_m2>0</kaelder_m2><antal_skader_treaar>0</antal_skader_treaar><opvarmning>1</opvarmning><pool_m2>0</pool_m2><pool_placering>indendoers</pool_placering><vandstop>false</vandstop><hoejvandslukke>false</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>1</toiletbadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>false</svampinsekt><raad>false</raad><roerkablerstikledninger>false</roerkablerstikledninger><selvrisiko>2000</selvrisiko><bo_type>120</bo_type><ydervaeg_kode>2</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>1</varmeinstallation><erhverv_m2>0</erhverv_m2><lejligheder>1</lejligheder><overdaek_m2>0</overdaek_m2><tagetage_lovlig_m2>47</tagetage_lovlig_m2></hus></insurancerequest></request>";
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
        public void TestBIProduct15Sead()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.8\"><customer><alder>42</alder><postnr>3400</postnr><adresse>Hvidkløvervej 19, 5210 Odense NV</adresse><kvhx>26521040005000000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>fe2b0541-cd69-4076-b540-4c4a9958c028</sessionid><insurancerequest><bil><koersel_aar>15000</koersel_aar><model>Mondeo</model><modelaar>2008</modelaar><motor>2,0</motor><fabrikat>Ford</fabrikat><fabrikatkode>55</fabrikatkode><modelkode>550</modelkode><nypris>650000</nypris><hk>147</hk><modelvariantid>248362007</modelvariantid><variant></variant><typenavn>P</typenavn><egenvaegt>1047</egenvaegt><koereklarvaegt>1158</koereklarvaegt><dktypegodkendelsesnumre>14692-05; 53842-05; 51784-04</dktypegodkendelsesnumre><drivmiddel>Benzin</drivmiddel><kw>64</kw><stelnummer>WF0EXXGBBE8L32220</stelnummer><foerste_reg>2008</foerste_reg><reg_nr>GG35577</reg_nr><kid>1008301200818731</kid><ansvar_kasko>true</ansvar_kasko><selvrisiko>1000</selvrisiko><friskade>false</friskade><brugere_u25>false</brugere_u25><udvidetglas>false</udvidetglas><fastpraemie>false</fastpraemie><foererdaekning>false</foererdaekning><privateleaset>false</privateleaset><vejhjaelp>true</vejhjaelp><antalaarmedbilforsikring>15</antalaarmedbilforsikring><antalskader>0</antalskader><skade1>0</skade1><skade2>0</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort><rabatordningvalg>false</rabatordningvalg></bil></insurancerequest></request>";
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
        public void TestBIProduct16()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, Tarup, 5210 Odense NV</adresse><kvhx>46135020019000000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>eb152c50-ea11-4366-8c53-18cbd2f237a7</sessionid><insurancerequest><bil><koersel_aar>15000</koersel_aar><model>Mondeo</model><modelaar>2008</modelaar><motor>2,0</motor><fabrikat>Ford</fabrikat><fabrikatkode>18</fabrikatkode><modelkode>168</modelkode><nypris>357326</nypris><hk>145</hk><modelvariantid>278682008</modelvariantid><variant>2,0 Ghia 145HK 5d</variant><typenavn>WF0EXXGBBE8L32220</typenavn><egenvaegt>1350</egenvaegt><koereklarvaegt>1477</koereklarvaegt><dktypegodkendelsesnumre>54071-01; 53358-01; 52631-02; 50332-03</dktypegodkendelsesnumre><drivmiddel>Benzin</drivmiddel><kw>107</kw><stelnummer>WF0EXXGBBE8L32220</stelnummer><foerste_reg>2008-09-30</foerste_reg><reg_nr>GG35577</reg_nr><kid>1008301200818731</kid><ansvar_kasko>true</ansvar_kasko><selvrisiko>5000</selvrisiko><friskade>false</friskade><brugere_u25>false</brugere_u25><udvidetglas>true</udvidetglas><fastpraemie>false</fastpraemie><foererdaekning>false</foererdaekning><privateleaset>false</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>15</antalaarmedbilforsikring><antalskader>0</antalskader><skade1>0</skade1><skade2>0</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort></bil></insurancerequest></request>";
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
        public void TestBIProduct17()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>42</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, Tarup, 5210 Odense NV</adresse><kvhx>46135020019000000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>c29fd6b9-1f2d-4aff-be1a-009d641582a9</sessionid><insurancerequest><bil><koersel_aar>15000</koersel_aar><model>Mondeo</model><modelaar>2008</modelaar><motor>2,0</motor><fabrikat>Ford</fabrikat><fabrikatkode>18</fabrikatkode><modelkode>168</modelkode><nypris>357326</nypris><hk>145</hk><modelvariantid>278682008</modelvariantid><variant>2,0 Ghia 145HK 5d</variant><typenavn>WF0EXXGBBE8L32220</typenavn><egenvaegt>1350</egenvaegt><koereklarvaegt>1477</koereklarvaegt><dktypegodkendelsesnumre>54071-01; 53358-01; 52631-02; 50332-03</dktypegodkendelsesnumre><drivmiddel>Benzin</drivmiddel><kw>107</kw><stelnummer>WF0EXXGBBE8L32220</stelnummer><foerste_reg>2008-09-30</foerste_reg><reg_nr>GG35577</reg_nr><kid>1008301200818731</kid><ansvar_kasko>true</ansvar_kasko><selvrisiko>5000</selvrisiko><friskade>false</friskade><brugere_u25>false</brugere_u25><udvidetglas>false</udvidetglas><fastpraemie>false</fastpraemie><foererdaekning>false</foererdaekning><privateleaset>false</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>15</antalaarmedbilforsikring><antalskader>0</antalskader><skade1>0</skade1><skade2>0</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort></bil></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Console.WriteLine(xmlRequest);
            Console.WriteLine(xmlResponse);
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

        private void writeXML(string XML, string testName)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\temp\\" + testName + ".xml");
            file.WriteLine(XML);
            file.Close();
        }
    }
}
//<request xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" schemaVersion="1.14"><customer><alder>40</alder><postnr>4000</postnr><adresse>Frederiksborgvej 5, 4000 Roskilde</adresse><kvhx>26521040005000000</kvhx></customer><insurancerequestsforcalculation>bil</insurancerequestsforcalculation><sessionid>815ba875-0f51-4b63-896b-a4b95c794bb4,Seka</sessionid><insurancerequest><bil><koersel_aar>15000</koersel_aar><model>Yaris</model><modelaar>2007</modelaar><motor>1,4</motor><fabrikat>Toyota</fabrikat><fabrikatkode>55</fabrikatkode><modelkode>550</modelkode><nypris>232242</nypris><hk>90</hk><modelvariantid>248382007</modelvariantid><variant>1,4 D-4D Linea Sol MMT 90HK 3d</variant><typenavn>VNKKC96330A105414</typenavn><egenvaegt>1047</egenvaegt><koereklarvaegt>1158</koereklarvaegt><dktypegodkendelsesnumre>14692-05; 53842-05; 51784-04</dktypegodkendelsesnumre><drivmiddel>Diesel</drivmiddel><kw>64</kw><stelnummer>VNKKC96330A105414</stelnummer><foerste_reg>2007-06-03</foerste_reg><reg_nr>AB61915</reg_nr><kid>1026501200715732</kid><ansvar_kasko>true</ansvar_kasko><selvrisiko>5000</selvrisiko><friskade>false</friskade><brugere_u25>false</brugere_u25><udvidetglas>true</udvidetglas><fastpraemie>true</fastpraemie><foererdaekning>false</foererdaekning><privateleaset>true</privateleaset><vejhjaelp>false</vejhjaelp><antalaarmedbilforsikring>10</antalaarmedbilforsikring><antalskader>2</antalskader><skade1>2013</skade1><skade2>2011</skade2><skade3>0</skade3><klipikoerekort>false</klipikoerekort><rabatordningvalg /></bil></insurancerequest></request>