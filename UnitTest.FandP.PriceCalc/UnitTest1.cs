using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.FandP.PriceCalc
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.16\">   <customer>     <alder>41</alder>     <postnr>9200</postnr>     <adresse>Johan Skjoldborgs Vej 9, 9200 Aalborg SV</adresse>     <kvhx>85138240009000000</kvhx>   </customer>   <insurancerequestsforcalculation>bil</insurancerequestsforcalculation>   <sessionid>4b224aaa-edad-48f9-a3c1-7ae0cf017f1a</sessionid>   <insurancerequest>     <bil>       <koersel_aar>15000</koersel_aar>       <model>Fabia</model>       <modelaar>2013</modelaar>       <motor>1,2</motor>       <fabrikat>Skoda</fabrikat>       <fabrikatkode>50</fabrikatkode>       <modelkode>695</modelkode>       <nypris>199001</nypris>       <hk>86</hk>       <modelvariantid>485512013</modelvariantid>       <variant>1,2 TSI Elegance 86HK 5d</variant>       <typenavn>TMBJM65J0D3100021</typenavn>       <egenvaegt>1050</egenvaegt>       <koereklarvaegt>1166</koereklarvaegt>       <dktypegodkendelsesnumre>55560-01; 54744-01; 53433-07; 55560-04; 54744-04</dktypegodkendelsesnumre>       <drivmiddel>Benzin</drivmiddel>       <kw>63</kw>       <stelnummer>TMBJM65J0D3100021</stelnummer>       <foerste_reg>2013-02-07</foerste_reg>       <reg_nr>AD40420</reg_nr>       <kid>9000000000278951</kid>       <ansvar_kasko>true</ansvar_kasko>       <selvrisiko>5000</selvrisiko>       <friskade>false</friskade>       <brugere_u25>false</brugere_u25>       <udvidetglas>false</udvidetglas>       <fastpraemie>false</fastpraemie>       <foererdaekning>false</foererdaekning>       <privateleaset>false</privateleaset>       <vejhjaelp>false</vejhjaelp>       <antalaarmedbilforsikring>20</antalaarmedbilforsikring>       <antalskader>0</antalskader>       <skade1>0</skade1>       <skade2>0</skade2>       <skade3>0</skade3>       <klipikoerekort>false</klipikoerekort>       <rabatordningvalg />     </bil>   </insurancerequest> <affinity_no>0</affinity_no></request>";
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.16\">   <customer>     <alder>41</alder>     <postnr>9200</postnr>     <adresse>Johan Skjoldborgs Vej 9, 9200 Aalborg SV</adresse>     <kvhx>85138240009000000</kvhx>   </customer>   <insurancerequestsforcalculation>ulykke</insurancerequestsforcalculation>   <sessionid>c56df515-ef55-470f-9f0c-de96eb51e55a</sessionid>   <insurancerequest>     <ulykke>       <stillingsbetegnelse>IT programmør</stillingsbetegnelse>       <stillingsbetegnelseId>672</stillingsbetegnelseId>       <heltidsulykke>true</heltidsulykke>       <forsikringssum_varigt_men>3000000</forsikringssum_varigt_men>       <forsikringssum_doed>100000</forsikringssum_doed>       <dobbelterstatning>true</dobbelterstatning>       <farligsport>false</farligsport>       <motorcykelknallert>false</motorcykelknallert>       <strakserstatning>false</strakserstatning>     </ulykke>   </insurancerequest> <affinity_no>522</affinity_no></request>";
            string xmlResponse;
            string error;

            PriceCalc.PriceCalcClient.PriceCalcServiceClient client = new PriceCalc.PriceCalcClient.PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(out xmlResponse, out error, xmlRequest));
            Assert.IsNotNull(xmlResponse);
            Assert.IsNotNull(error);
        }
    }
}
