using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient;
//using UnitTest.FandP.PriceCalc.ReferenceServiceClient;

using System.Xml;
using System.IO;


namespace UnitTest.FandP.PriceCalculator
{
    [TestClass]
    public class UnitTestProdSO
    {
        StreamWriter w = File.AppendText("L:\\CustomErrors\\svc_FandP.PriceCalc\\Dump.log");


        [TestMethod]
        public void TestSOProduct01()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>42</alder><postnr>0</postnr></customer><insurancerequestsforcalculation>fritidshus</insurancerequestsforcalculation><sessionid>75ab072a-2c12-4834-aae8-3dd323a38720</sessionid><insurancerequest><fritidshus><fritidshus_adresse>Strandgårdsvej 97, Bro Mark, 5464 Brenderup Fyn</fritidshus_adresse><fritidshus_kvhx>41011490097000000</fritidshus_kvhx><fritidshus_postnr>5464</fritidshus_postnr><opfoerelsesaar>2005</opfoerelsesaar><bebygget_m2>144</bebygget_m2><bolig_m2>198</bolig_m2><garage_m2>8</garage_m2><etager>2</etager><toiletbadrum>2</toiletbadrum><opvarmning>1</opvarmning><braendeovn>false</braendeovn><tagbeklaedning>4</tagbeklaedning><straatag_brandisoleret>false</straatag_brandisoleret><nedlagt_landbrug>false</nedlagt_landbrug><pool_placering>indendoers</pool_placering><pool_m2>0</pool_m2><indbosum>200000</indbosum><selvrisiko>2000</selvrisiko><svampinsekt>true</svampinsekt><raad>false</raad><roerstikledninger>false</roerstikledninger><kosmetiskdaekning>false</kosmetiskdaekning><lejerskader>false</lejerskader><elektronik>false</elektronik><udlejes>false</udlejes><antal_skader_treaar>0</antal_skader_treaar><bo_type>510</bo_type><ydervaeg_kode>5</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>7</varmeinstallation><erhverv_m2>0</erhverv_m2><lejligheder>0</lejligheder><overdaek_m2>0</overdaek_m2><tagetage_lovlig_m2>62</tagetage_lovlig_m2><kaelder_m2>0</kaelder_m2></fritidshus></insurancerequest></request>";
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
        public void Slåenvej_3_Tørresø_Strand_5450_Otterup()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>42</alder><postnr>0</postnr><adresse /><kvhx /></customer><insurancerequestsforcalculation>fritidshus</insurancerequestsforcalculation><sessionid>34c236a3-bd3a-4754-9a4f-d5d4247b309e</sessionid><insurancerequest><fritidshus><fritidshus_adresse /><fritidshus_kvhx>48011260003000000</fritidshus_kvhx><fritidshus_postnr>5450</fritidshus_postnr><opfoerelsesaar>1989</opfoerelsesaar><bebygget_m2>54</bebygget_m2><bolig_m2>59</bolig_m2><garage_m2>100</garage_m2><etager>2</etager><toiletbadrum>1</toiletbadrum><opvarmning>1</opvarmning><braendeovn>false</braendeovn><tagbeklaedning>3</tagbeklaedning><straatag_brandisoleret>false</straatag_brandisoleret><nedlagt_landbrug>false</nedlagt_landbrug><pool_placering>indendoers</pool_placering><pool_m2>0</pool_m2><indbosum>200000</indbosum><selvrisiko>2000</selvrisiko><svampinsekt>false</svampinsekt><raad>false</raad><roerstikledninger>false</roerstikledninger><kosmetiskdaekning>false</kosmetiskdaekning><lejerskader>false</lejerskader><elektronik>false</elektronik><udlejes>false</udlejes><antal_skader_treaar>0</antal_skader_treaar><bo_type>510</bo_type><ydervaeg_kode>5</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>7</varmeinstallation><erhverv_m2>0</erhverv_m2><lejligheder>0</lejligheder><overdaek_m2>0</overdaek_m2><tagetage_lovlig_m2>5</tagetage_lovlig_m2><kaelder_m2>0</kaelder_m2></fritidshus></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            w.WriteLine("****************************************************************************************");
            w.WriteLine(xmlRequest);
            w.WriteLine("---------------------------------------------------------------------------------------");
            w.WriteLine(xmlResponse);
            w.WriteLine("****************************************************************************************");
            w.Close();
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);          
        }


        
        [TestMethod]
        public void TestSOProduct02()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>45</alder><postnr>0</postnr><adresse /><kvhx /></customer><insurancerequestsforcalculation>fritidshus</insurancerequestsforcalculation><sessionid>68081422-72e7-42f9-be45-237de9e65aa6</sessionid><insurancerequest><fritidshus><fritidshus_adresse /><fritidshus_kvhx /><fritidshus_postnr>5400</fritidshus_postnr><opfoerelsesaar>1976</opfoerelsesaar><bebygget_m2>145</bebygget_m2><bolig_m2>145</bolig_m2><garage_m2>75</garage_m2><etager>1</etager><toiletbadrum>2</toiletbadrum><opvarmning>51</opvarmning><braendeovn>false</braendeovn><tagbeklaedning>5</tagbeklaedning><straatag_brandisoleret>false</straatag_brandisoleret><nedlagt_landbrug>false</nedlagt_landbrug><pool_placering>indendoers</pool_placering><pool_m2>0</pool_m2><indbosum>200000</indbosum><selvrisiko>2000</selvrisiko><svampinsekt>true</svampinsekt><raad>false</raad><roerstikledninger>false</roerstikledninger><kosmetiskdaekning>false</kosmetiskdaekning><lejerskader>false</lejerskader><elektronik>false</elektronik><udlejes>false</udlejes><antal_skader_treaar>2</antal_skader_treaar><bo_type>120</bo_type><ydervaeg_kode>1</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>1</varmeinstallation><erhverv_m2>0</erhverv_m2><lejligheder>1</lejligheder><overdaek_m2>0</overdaek_m2><tagetage_lovlig_m2>0</tagetage_lovlig_m2><kaelder_m2>40</kaelder_m2></fritidshus></insurancerequest></request>";
                //"<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>42</alder><postnr>0</postnr><adresse /><kvhx /></customer><insurancerequestsforcalculation>fritidshus</insurancerequestsforcalculation><sessionid>34c236a3-bd3a-4754-9a4f-d5d4247b309e</sessionid><insurancerequest><fritidshus><fritidshus_adresse /><fritidshus_kvhx>48011260003000000</fritidshus_kvhx><fritidshus_postnr>5450</fritidshus_postnr><opfoerelsesaar>1989</opfoerelsesaar><bebygget_m2>54</bebygget_m2><bolig_m2>59</bolig_m2><garage_m2>100</garage_m2><etager>2</etager><toiletbadrum>1</toiletbadrum><opvarmning>1</opvarmning><braendeovn>false</braendeovn><tagbeklaedning>3</tagbeklaedning><straatag_brandisoleret>false</straatag_brandisoleret><nedlagt_landbrug>false</nedlagt_landbrug><pool_placering>indendoers</pool_placering><pool_m2>0</pool_m2><indbosum>200000</indbosum><selvrisiko>2000</selvrisiko><svampinsekt>false</svampinsekt><raad>false</raad><roerstikledninger>false</roerstikledninger><kosmetiskdaekning>false</kosmetiskdaekning><lejerskader>false</lejerskader><elektronik>false</elektronik><udlejes>false</udlejes><antal_skader_treaar>0</antal_skader_treaar><bo_type>510</bo_type><ydervaeg_kode>5</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>7</varmeinstallation><erhverv_m2>0</erhverv_m2><lejligheder>0</lejligheder><overdaek_m2>0</overdaek_m2><tagetage_lovlig_m2>5</tagetage_lovlig_m2><kaelder_m2>0</kaelder_m2></fritidshus></insurancerequest></request>";
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            w.WriteLine("****************************************************************************************");
            w.WriteLine(xmlRequest);
            w.WriteLine("---------------------------------------------------------------------------------------");
            w.WriteLine(xmlResponse);
            w.WriteLine("****************************************************************************************");
            w.Close();
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);          
        }


        [TestMethod]
        public void TestSOProduct03()
        {
            string xmlRequest = "<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>43</alder><postnr>0</postnr></customer><insurancerequestsforcalculation>fritidshus</insurancerequestsforcalculation><sessionid>be83a7b8-4318-4d24-af97-9ccecb1cc77e</sessionid><insurancerequest><fritidshus><fritidshus_adresse>Kukkenbjerg 28, Svanninge, 5600 Faaborg</fritidshus_adresse><fritidshus_kvhx>43007810028000000</fritidshus_kvhx><fritidshus_postnr>5600</fritidshus_postnr><opfoerelsesaar>2010</opfoerelsesaar><bebygget_m2>190</bebygget_m2><bolig_m2>190</bolig_m2><garage_m2>0</garage_m2><etager>1</etager><toiletbadrum>1</toiletbadrum><opvarmning>1</opvarmning><braendeovn>false</braendeovn><tagbeklaedning>7</tagbeklaedning><straatag_brandisoleret>false</straatag_brandisoleret><nedlagt_landbrug>false</nedlagt_landbrug><pool_placering>indendoers</pool_placering><pool_m2>0</pool_m2><indbosum>600000</indbosum><selvrisiko>2000</selvrisiko><svampinsekt>false</svampinsekt><raad>false</raad><roerstikledninger>false</roerstikledninger><kosmetiskdaekning>false</kosmetiskdaekning><lejerskader>false</lejerskader><elektronik>false</elektronik><udlejes>false</udlejes><antal_skader_treaar>0</antal_skader_treaar><bo_type>120</bo_type><ydervaeg_kode>4</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>7</varmeinstallation><erhverv_m2>0</erhverv_m2><lejligheder>1</lejligheder><overdaek_m2>0</overdaek_m2><tagetage_lovlig_m2>0</tagetage_lovlig_m2><kaelder_m2>0</kaelder_m2></fritidshus></insurancerequest></request>";
            //<request xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" schemaVersion=\"1.14\"><customer><alder>43</alder><postnr>0</postnr></customer><insurancerequestsforcalculation>fritidshus</insurancerequestsforcalculation><sessionid>be83a7b8-4318-4d24-af97-9ccecb1cc77e</sessionid><insurancerequest><fritidshus><fritidshus_adresse>Kukkenbjerg 28, Svanninge, 5600 Faaborg</fritidshus_adresse><fritidshus_kvhx>43007810028000000</fritidshus_kvhx><fritidshus_postnr>5600</fritidshus_postnr><opfoerelsesaar>1877</opfoerelsesaar><bebygget_m2>190</bebygget_m2><bolig_m2>190</bolig_m2><garage_m2>0</garage_m2><etager>1</etager><toiletbadrum>1</toiletbadrum><opvarmning>1</opvarmning><braendeovn>false</braendeovn><tagbeklaedning>7</tagbeklaedning><straatag_brandisoleret>false</straatag_brandisoleret><nedlagt_landbrug>false</nedlagt_landbrug><pool_placering>indendoers</pool_placering><pool_m2>0</pool_m2><indbosum>600000</indbosum><selvrisiko>2000</selvrisiko><svampinsekt>false</svampinsekt><raad>false</raad><roerstikledninger>false</roerstikledninger><kosmetiskdaekning>false</kosmetiskdaekning><lejerskader>false</lejerskader><elektronik>false</elektronik><udlejes>false</udlejes><antal_skader_treaar>0</antal_skader_treaar><bo_type>120</bo_type><ydervaeg_kode>4</ydervaeg_kode><kaelder_lovlig_m2>0</kaelder_lovlig_m2><varmeinstallation>7</varmeinstallation><erhverv_m2>0</erhverv_m2><lejligheder>1</lejligheder><overdaek_m2>0</overdaek_m2><tagetage_lovlig_m2>0</tagetage_lovlig_m2><kaelder_m2>0</kaelder_m2></fritidshus></insurancerequest></request>
            string xmlResponse;
            string error;

            PriceCalcServiceClient client = new PriceCalcServiceClient();
            Assert.IsTrue(client.CalculatePrice(xmlRequest, out xmlResponse, out error));
            Assert.IsNotNull(xmlResponse);
            Assert.IsNull(error);          
        }
    }
}
