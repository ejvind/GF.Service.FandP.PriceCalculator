using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient;
//using UnitTest.FandP.PriceCalc.ReferenceServiceClient;

using System.Xml;

namespace UnitTest.FandP.PriceCalculator
{
    [TestClass]
    public class UnitTestProdIN
    {
        /*
         * Artifact artf290445 : Der beregnes for Nedlagt Landbrug
         * 
         * Ret oplysninger på Bramgåsvej 2A, 4760 til nedlagt landbrug.
        */

        [TestMethod]
        public void TestINProduct01()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>23</alder><postnr>6400</postnr><adresse>Koralvej 8, 2. mf, 6400 Sønderborg</adresse><kvhx>540102500080200mf</kvhx></customer><insurancerequestsforcalculation>indbo</insurancerequestsforcalculation><sessionid>be83a7b8-4318-4d24-af97-9ccecb1cc77e</sessionid><insurancerequest><indbo><antal_voksne>1</antal_voksne><antal_boern>0</antal_boern><bolig_m2>53</bolig_m2><antal_skader_treaar>0</antal_skader_treaar><kaelder_m2>0</kaelder_m2><tagbeklaedning>3</tagbeklaedning><straatag_brandisoleret>false</straatag_brandisoleret><eksisterende_forsikring>true</eksisterende_forsikring><alarm>false</alarm><tilknyttet_alarmcentral>false</tilknyttet_alarmcentral><vindueslaas>false</vindueslaas><roegalarm>true</roegalarm><glaskumme>false</glaskumme><pludseligskade>false</pludseligskade><elektronik>false</elektronik><rejseverden>false</rejseverden><rejseeuropa>false</rejseeuropa><afbestillingsforsikring>false</afbestillingsforsikring><indbosum>1000000</indbosum><selvrisiko>2000</selvrisiko><bo_type>140</bo_type><ydervaeg_kode>90</ydervaeg_kode><erhverv_m2>0</erhverv_m2><lejligheder>27</lejligheder></indbo></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);
        }

    }
}
