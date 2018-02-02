using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitTest.FandP.PriceCalc;

namespace UnitTest.FandP.PriceCalc
{
    [TestClass]
    public class ARKINT_211
    {
        [TestMethod]
        public void Arkint_211_HS01()
        {
            string xmlRequest = "<?xml version=\"1.0\" encoding=\"utf-8\"?><request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>44</alder><postnr>3550</postnr><adresse>Kongensgade 11, 3550 Slangerup</adresse><kvhx>25006510011000000</kvhx></customer><insurancerequestsforcalculation>hus</insurancerequestsforcalculation><sessionid>424eb99a-3642-4254-bbc4-59eaf303116f</sessionid><insurancerequest><hus><opfoerelsesaar>1924</opfoerelsesaar><bebygget_m2>140</bebygget_m2><bolig_m2>344</bolig_m2><garage_m2>0</garage_m2><etager>3</etager><tagbeklaedning>5</tagbeklaedning><kaelder_m2>30</kaelder_m2><antal_skader_treaar>0</antal_skader_treaar><opvarmning>7</opvarmning><pool_m2>0</pool_m2><pool_placering>indendoers</pool_placering><vandstop>false</vandstop><hoejvandslukke>false</hoejvandslukke><nedlagt_landbrug>false</nedlagt_landbrug><toiletbadrum>1</toiletbadrum><straatag_brandisoleret>false</straatag_brandisoleret><udvidetvand>false</udvidetvand><dyrskader>false</dyrskader><kosmetiskdaekning>false</kosmetiskdaekning><svampinsekt>true</svampinsekt><raad>false</raad><roerkablerstikledninger>true</roerkablerstikledninger><selvrisiko>2000</selvrisiko><bo_type>140</bo_type><ydervaeg_kode>1</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>2</varmeinstallation><erhverv_m2>140</erhverv_m2><lejligheder>1</lejligheder><overdaek_m2>39</overdaek_m2><tagetage_lovlig_m2>70</tagetage_lovlig_m2></hus></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalc.PriceCalcClient.PriceCalcServiceClient client = new PriceCalc.PriceCalcClient.PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(out xmlResponse, out error, xmlRequest));
            Assert.IsNotNull(xmlResponse);
            Assert.IsNotNull(error);
        }

        [TestMethod]
        public void Arkint_211_SO01()
        {
        }

        [TestMethod]
        public void Integra_502IN01()
        {
            string xmlRequest = "<?xml version=\"1.0\" encoding=\"utf-8\"?><request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.15\"><customer><alder>45</alder><postnr>5210</postnr><adresse>Hvidkløvervej 19, Tarup, 5210 Odense NV</adresse><kvhx>46135020019000000</kvhx></customer><insurancerequestsforcalculation>indbo</insurancerequestsforcalculation><sessionid>d41f8a00-9983-408f-b201-519a050a0219</sessionid><insurancerequest><indbo><antal_voksne>2</antal_voksne><antal_boern>1</antal_boern><bolig_m2>127</bolig_m2><antal_skader_treaar>0</antal_skader_treaar><kaelder_m2>0</kaelder_m2><tagbeklaedning>5</tagbeklaedning><straatag_brandisoleret>false</straatag_brandisoleret><eksisterende_forsikring>true</eksisterende_forsikring><alarm>false</alarm><tilknyttet_alarmcentral>false</tilknyttet_alarmcentral><vindueslaas>false</vindueslaas><roegalarm>true</roegalarm><glaskumme>false</glaskumme><pludseligskade>false</pludseligskade><elektronik>false</elektronik><rejseverden>false</rejseverden><rejseeuropa>false</rejseeuropa><afbestillingsforsikring>false</afbestillingsforsikring><indbosum>1000000</indbosum><selvrisiko>2000</selvrisiko><bo_type>120</bo_type><ydervaeg_kode>90</ydervaeg_kode><erhverv_m2>0</erhverv_m2><lejligheder>1</lejligheder></indbo></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalc.PriceCalcClient.PriceCalcServiceClient client = new PriceCalc.PriceCalcClient.PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(out xmlResponse, out error, xmlRequest));
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }
    }
}
