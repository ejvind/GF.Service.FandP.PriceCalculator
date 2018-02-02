using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Xml.Serialization;

using GF.Service.FandP.PriceCalc.DOM;
using GF.Service.FandP.PriceCalculator.TIABBRClient;
using GF.Service.FandP.PriceCalculator.TIAPriceCalcClient;
using GF.Service.FandP.PriceCalculator.TIAReferenceClient;
using GF.Service.FandP.PriceCalculator.EmailClient;
using GF.Components.ErrorHandling;
using System.Configuration;
using System.Net;

namespace GF.Service.FandP.PriceCalc.Helpers
{
    public class PriceCalcConverter
    {
        #region Public Methods

        public static bool Serialize<T>(T objectToSerialize, out string xml, out Exception exc)
        {
            xml = null;
            XmlSerializer serializer = null;
            StringWriter writer = null;

            try
            {
                serializer = new XmlSerializer(typeof(T));
                writer = new StringWriter(new StringBuilder(xml));
                serializer.Serialize(writer, objectToSerialize);
                xml = writer.ToString();
            }
            catch (Exception serExc)
            {
                exc = new Exception("Error when trying to serialize file: ", serExc);
                xml = null;
                return false;
            }
            finally
            {
                // Clean up
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
                if (serializer != null)
                {
                    serializer = null;
                }
            }

            exc = null;
            return true;
        }

        public static bool Deserialize<T>(string xml, out T deserializedObject, out Exception exc)
        {
            XmlSerializer serializer = null;
            TextReader reader = null;

            try
            {
                serializer = new XmlSerializer(typeof(T));
                reader = new StringReader(xml);
                deserializedObject = (T)serializer.Deserialize(reader);
            }
            catch (Exception deserExc)
            {
                exc = new Exception("Error when trying to deserialize xml file", deserExc);
                deserializedObject = default(T);
                return false;
            }
            finally
            {
                // Clean up
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                if (serializer != null)
                {
                    serializer = null;
                }
            }

            exc = null;
            return true;
        }

        public static bool ConvertBilRequest(request FPRequest, out BI_request GFRequest, out string error, out bool addGFHelp)
        {            
            int newSelvrisko = 0;
            error = null;
            addGFHelp = false;

            try
            {
                TIAReferenceServiceClient client = new TIAReferenceServiceClient();

                tariff_codes_request drivmiddel_req = new tariff_codes_request();
                tariff_codes_response drivmiddel_res = new tariff_codes_response();

                tariff_codes_request selvrisko_req = new tariff_codes_request();
                tariff_codes_response selvrisko_res = new tariff_codes_response();

                gf_club_request club_req = new gf_club_request();
                gf_club_response club_res = new gf_club_response();

                drivmiddel_req.product_line_id = "BI";
                drivmiddel_req.table_name = "XDRIVMIDDEL";
                drivmiddel_req.order_by = "CODE";
                drivmiddel_req.indexed = "N";
                if (!client.GetTariffCode(drivmiddel_req, out drivmiddel_res, out error))
                {
                    GFRequest = null;
                    return false;
                }
                else if (drivmiddel_res.error_record != null)
                {
                    error = drivmiddel_res.error_record.error_description;
                    GFRequest = null;
                    return false;
                }

                selvrisko_req.product_line_id = "BI";
                selvrisko_req.table_name = "XSELVRISIKO";
                selvrisko_req.order_by = "MAX_VALUE";
                selvrisko_req.indexed = "Y";
                if (!client.GetTariffCode(selvrisko_req, out selvrisko_res, out error))
                {
                    GFRequest = null;
                    return false;
                }
                else if (selvrisko_res.error_record != null)
                {
                    error = selvrisko_res.error_record.error_description;
                    GFRequest = null;
                    return false;
                }


                club_req.zip_code = Convert.ToString(FPRequest.customer.postnr);
                club_req.company_profession_code = "0";
                if (!client.GetClubNumber(club_req, out club_res, out error))
                {
                    GFRequest = null;
                    return false;
                }

                GFRequest = new BI_request();
                GFRequest.BIproduct = new BIproduct();
                GFRequest.BIproduct.BIproductLine = new BIproductLine();
                GFRequest.BIproduct.BIproductLine.BIobject = new BIobject();
                GFRequest.BIproduct.BIproductLine.BIobject.parameters = new parameters_bil();
                GFRequest.BIproduct.BIproductLine.BIobject.risk = new risk_bil();

                GFRequest.BIproduct.BIproductLine.affinity = FPRequest.affinity_no;
                GFRequest.BIproduct.BIproductLine.affinitySpecified = true;
                GFRequest.BIproduct.BIproductLine.BIobject.parameters.include_member_deposit = "N";
                GFRequest.BIproduct.BIproductLine.BIobject.parameters.postal_code = (int)FPRequest.customer.postnr;                
                GFRequest.BIproduct.BIproductLine.BIobject.parameters.car_registration_number = FPRequest.insurancerequest.bil.reg_nr;
                
                /*if (FPRequest.insurancerequest.bil.friskade || FPRequest.insurancerequest.bil.foererdaekning || FPRequest.insurancerequest.bil.udvidetglas)
                {
                    GFRequest.BIproduct.BIproductLine.BIobject.parameters.car_art = 7;
                }
                else
                {
                    GFRequest.BIproduct.BIproductLine.BIobject.parameters.car_art = 6;
                }*/

                GFRequest.BIproduct.BIproductLine.BIobject.parameters.car_art = 6;

                GFRequest.BIproduct.BIproductLine.BIobject.parameters.car_manufacturer = FPRequest.insurancerequest.bil.fabrikat;
                GFRequest.BIproduct.BIproductLine.BIobject.parameters.car_model_type = FPRequest.insurancerequest.bil.model;
                GFRequest.BIproduct.BIproductLine.BIobject.parameters.car_variant = FPRequest.insurancerequest.bil.variant;
                GFRequest.BIproduct.BIproductLine.BIobject.parameters.car_horsepower = FPRequest.insurancerequest.bil.hk;
                GFRequest.BIproduct.BIproductLine.BIobject.parameters.car_value = FPRequest.insurancerequest.bil.nypris > 0 ? (int)FPRequest.insurancerequest.bil.nypris : 250000; //no car value (zero) is changed to the static value 250.000 according to INTEGRA-547.
                GFRequest.BIproduct.BIproductLine.BIobject.parameters.car_weight = FPRequest.insurancerequest.bil.egenvaegt;

                for (int i = 0; i < drivmiddel_res.tariff_code_record.Length; i++)
                {
                    if (FPRequest.insurancerequest.bil.drivmiddel.ToUpper() == drivmiddel_res.tariff_code_record[i].description.ToUpper())
                    {
                        GFRequest.BIproduct.BIproductLine.BIobject.parameters.propellant = Convert.ToInt32(drivmiddel_res.tariff_code_record[i].code);
                    }
                }

                GFRequest.BIproduct.BIproductLine.BIobject.parameters.car_age = DateTime.Now.Year - FPRequest.insurancerequest.bil.foerste_reg.Year;
                GFRequest.BIproduct.BIproductLine.BIobject.parameters.atv = yes_no.N;
                GFRequest.BIproduct.BIproductLine.BIobject.parameters.year_own_car = FPRequest.insurancerequest.bil.antalaarmedbilforsikring;
                GFRequest.BIproduct.BIproductLine.BIobject.parameters.damages_last_years = FPRequest.insurancerequest.bil.antalskader;
                GFRequest.BIproduct.BIproductLine.BIobject.parameters.mileage_yearly = (int)FPRequest.insurancerequest.bil.koersel_aar/1000;
                                
                if (FPRequest.insurancerequest.bil.selvrisiko > Convert.ToUInt16(selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].max_val))
                {
                    newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].max_val);
                    GFRequest.BIproduct.BIproductLine.BIobject.parameters.car_general_excess = newSelvrisko;
                }
                else
                {
                    /*for (int i = selvrisko_res.tariff_code_record.Length; i > 0; i--)
                    {
                        if (FPRequest.insurancerequest.bil.selvrisiko <= Convert.ToUInt16(selvrisko_res.tariff_code_record[i - 1].max_val) && Convert.ToUInt16(selvrisko_res.tariff_code_record[i - 1].max_val) > 0)
                        {
                            newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val);
                            GFRequest.BIproduct.BIproductLine.BIobject.parameters.car_general_excess = newSelvrisko;
                        }
                    }*/
                    for (int i = 0; i < selvrisko_res.tariff_code_record.Length; i++)
                    {
                        if (FPRequest.insurancerequest.bil.selvrisiko >= Convert.ToUInt16(selvrisko_res.tariff_code_record[i].max_val) && FPRequest.insurancerequest.bil.selvrisiko <= Convert.ToUInt16(selvrisko_res.tariff_code_record[i+1].max_val))
                        {
                            if (Math.Abs(FPRequest.insurancerequest.bil.selvrisiko - Convert.ToUInt16(selvrisko_res.tariff_code_record[i].max_val)) < Math.Abs(FPRequest.insurancerequest.bil.selvrisiko - Convert.ToUInt16(selvrisko_res.tariff_code_record[i + 1].max_val)))
                            {
                                newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i].max_val);
                            }
                            else
                            {
                                newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i+1].max_val);
                            }
                            GFRequest.BIproduct.BIproductLine.BIobject.parameters.car_general_excess = newSelvrisko;
                            break;
                        }
                    }
                }

                BbrDataResponse bbrResponse;                
                string newKVHX;
                bbrResponse = GetBBRData(FPRequest.customer.kvhx, out newKVHX);

                if (bbrResponse.BbrDataEntries.Length > 0)
                {
                    try
                    {
                        GFRequest.BIproduct.BIproductLine.BIobject.parameters.tariff_user_street = bbrResponse.BbrDataEntries[0].Vejnavn;
                    }
                    catch (Exception)
                    {
                        error = "Streetname not found from KVHX";
                        GFRequest = null;
                        return false;
                    }                    
                }                

                GFRequest.BIproduct.BIproductLine.BIobject.parameters.tariff_user_age = FPRequest.customer.alder;

                GFRequest.BIproduct.BIproductLine.BIobject.parameters.leasing_car = FPRequest.insurancerequest.bil.privateleaset ? "J" : "N";
                
                GFRequest.BIproduct.BIproductLine.BIobject.parameters.extra_equipment = 0;
                GFRequest.BIproduct.BIproductLine.BIobject.parameters.addAllRisks = yes_no.N;

                GFRequest.BIproduct.BIproductLine.BIobject.risk.ansvar = new risk_generic();
                GFRequest.BIproduct.BIproductLine.BIobject.risk.ansvar.risk_sum = 0;
                GFRequest.BIproduct.BIproductLine.BIobject.risk.ansvar.risk_sumSpecified = true;
                GFRequest.BIproduct.BIproductLine.BIobject.risk.ansvar.risk_excess = newSelvrisko;
                GFRequest.BIproduct.BIproductLine.BIobject.risk.ansvar.risk_excessSpecified = true;
                GFRequest.BIproduct.BIproductLine.BIobject.risk.ansvar.risk_yn = yes_no.Y;
                GFRequest.BIproduct.BIproductLine.BIobject.risk.ansvar.risk_ynSpecified = true;

                if (FPRequest.insurancerequest.bil.ansvar_kasko)
                {
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.kasko = new risk_generic();
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.kasko.risk_sum = (int)FPRequest.insurancerequest.bil.nypris;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.kasko.risk_sumSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.kasko.risk_excess = newSelvrisko;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.kasko.risk_excessSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.kasko.risk_yn = yes_no.Y;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.kasko.risk_ynSpecified = true;
                }
                else
                {
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.kasko = new risk_generic();
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.kasko.risk_sum = 0;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.kasko.risk_sumSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.kasko.risk_excess = 0;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.kasko.risk_excessSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.kasko.risk_yn = yes_no.N;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.kasko.risk_ynSpecified = true;
                }

                //if (GFRequest.BIproduct.BIproductLine.BIobject.parameters.car_art == 7)
                if (FPRequest.insurancerequest.bil.foererdaekning)
                {
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererpladsdaekning = new risk_generic();
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererpladsdaekning.risk_sum = 0;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererpladsdaekning.risk_sumSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererpladsdaekning.risk_excess = newSelvrisko;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererpladsdaekning.risk_excessSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererpladsdaekning.risk_yn = yes_no.Y;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererpladsdaekning.risk_ynSpecified = true;

                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_varigt_men = new risk_generic();
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_varigt_men.risk_sum = 0;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_varigt_men.risk_sumSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_varigt_men.risk_excess = newSelvrisko;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_varigt_men.risk_excessSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_varigt_men.risk_yn = yes_no.Y;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_varigt_men.risk_ynSpecified = true;

                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_doed = new risk_generic();
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_doed.risk_sum = 0;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_doed.risk_sumSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_doed.risk_excess = newSelvrisko;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_doed.risk_excessSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_doed.risk_yn = yes_no.Y;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_doed.risk_ynSpecified = true;
                }
                else
                {
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererpladsdaekning = new risk_generic();
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererpladsdaekning.risk_sum = 0;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererpladsdaekning.risk_sumSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererpladsdaekning.risk_excess = newSelvrisko;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererpladsdaekning.risk_excessSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererpladsdaekning.risk_yn = yes_no.N;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererpladsdaekning.risk_ynSpecified = true;

                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_varigt_men = new risk_generic();
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_varigt_men.risk_sum = 0;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_varigt_men.risk_sumSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_varigt_men.risk_excess = newSelvrisko;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_varigt_men.risk_excessSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_varigt_men.risk_yn = yes_no.N;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_varigt_men.risk_ynSpecified = true;

                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_doed = new risk_generic();
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_doed.risk_sum = 0;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_doed.risk_sumSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_doed.risk_excess = newSelvrisko;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_doed.risk_excessSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_doed.risk_yn = yes_no.N;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.foererulykke_doed.risk_ynSpecified = true;
                }

                if (FPRequest.insurancerequest.bil.friskade)
                {
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.friskade = new risk_generic();
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.friskade.risk_sum = 0;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.friskade.risk_sumSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.friskade.risk_excess = newSelvrisko;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.friskade.risk_excessSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.friskade.risk_yn = yes_no.Y;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.friskade.risk_ynSpecified = true;
                }
                else
                {
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.friskade = new risk_generic();
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.friskade.risk_sum = 0;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.friskade.risk_sumSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.friskade.risk_excess = newSelvrisko;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.friskade.risk_excessSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.friskade.risk_yn = yes_no.N;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.friskade.risk_ynSpecified = true;
                }


                if (FPRequest.insurancerequest.bil.udvidetglas)
                {
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.udvidet_glas = new risk_generic();
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.udvidet_glas.risk_sum = 0;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.udvidet_glas.risk_sumSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.udvidet_glas.risk_excess = newSelvrisko;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.udvidet_glas.risk_excessSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.udvidet_glas.risk_yn = yes_no.Y;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.udvidet_glas.risk_ynSpecified = true;
                }
                else
                {
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.udvidet_glas = new risk_generic();
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.udvidet_glas.risk_sum = 0;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.udvidet_glas.risk_sumSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.udvidet_glas.risk_excess = newSelvrisko;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.udvidet_glas.risk_excessSpecified = true;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.udvidet_glas.risk_yn = yes_no.N;
                    GFRequest.BIproduct.BIproductLine.BIobject.risk.udvidet_glas.risk_ynSpecified = true;
                }


                if (FPRequest.insurancerequest.bil.vejhjaelp && (club_res.transport_assistance == "Y" || club_res.transport_assistance == "N"))
                {
                    GFRequest.BIproduct.BIproductLine.BIobject.parameters.include_club_membership = "Y";
                    GFRequest.BIproduct.BIproductLine.BIobject.parameters.include_transport_assistance = "Y";
                    GFRequest.BIproduct.BIproductLine.BIobject.parameters.club_no = club_res.club_number;
                }
                else
                {
                    GFRequest.BIproduct.BIproductLine.BIobject.parameters.include_club_membership = "N";
                    GFRequest.BIproduct.BIproductLine.BIobject.parameters.include_transport_assistance = "N";
                    GFRequest.BIproduct.BIproductLine.BIobject.parameters.club_no = 0;
                }

                if (FPRequest.insurancerequest.bil.vejhjaelp && club_res.transport_assistance == "NONE")
                {
                    addGFHelp = true;
                }
            }

            catch (Exception exc)
            {
                ErrorsLogger.Error(new ErrorInfo(exc.ToString()));
                GFRequest = null;
                return false;
            }

            return true;
        }       

        public static bool ConvertBasisRequest(request FPRequest, out BA_request GFRequest, out string error)
        {
            int newSelvrisko = 0;
            long newIndbo = 0;
            error = null;

            try
            {
                TIAReferenceServiceClient client = new TIAReferenceServiceClient();

                tariff_codes_request indbo_sum_req = new tariff_codes_request();
                tariff_codes_response indbo_sum_res = new tariff_codes_response();

                tariff_codes_request selvrisko_req = new tariff_codes_request();
                tariff_codes_response selvrisko_res = new tariff_codes_response();

                indbo_sum_req.product_line_id = "BA";
                indbo_sum_req.table_name = "INDBO_SUM";
                indbo_sum_req.order_by = "MAX_VALUE";
                indbo_sum_req.indexed = "Y";
                if (!client.GetTariffCode(indbo_sum_req, out indbo_sum_res, out error))
                {
                    GFRequest = null;
                    return false;
                }
                else if (indbo_sum_res.error_record != null)
                {
                    error = indbo_sum_res.error_record.error_description;
                    GFRequest = null;
                    return false;
                }

                selvrisko_req.product_line_id = "BA";
                selvrisko_req.table_name = "SELVRISIKO";
                selvrisko_req.order_by = "MAX_VALUE";
                selvrisko_req.indexed = "Y";
                if (!client.GetTariffCode(selvrisko_req, out selvrisko_res, out error))
                {
                    GFRequest = null;
                    return false;
                }
                else if (selvrisko_res.error_record != null)
                {
                    error = selvrisko_res.error_record.error_description;
                    GFRequest = null;
                    return false;
                }

                if (FPRequest.insurancerequest.indbo.tagbeklaedning == RoofingMaterialCodeType.Item7)
                {
                    error = "GF tegner ikke forsikring for denne type tagbeklædning.";
                    GFRequest = null;
                    return false;
                }

                GFRequest = new BA_request();
                GFRequest.BAproduct = new BAproduct();
                GFRequest.BAproduct.BAproductLine = new BAproductLine();
                GFRequest.BAproduct.BAproductLine.BAobject = new BAobject();
                GFRequest.BAproduct.BAproductLine.BAobject.parameters = new parameters_BA();
                GFRequest.BAproduct.BAproductLine.BAobject.risk = new risk_ba();

                GFRequest.BAproduct.BAproductLine.affinity = FPRequest.affinity_no;
                GFRequest.BAproduct.BAproductLine.affinitySpecified = true;

                GFRequest.BAproduct.BAproductLine.BAobject.parameters.addAllRisks = yes_no.N;
                GFRequest.BAproduct.BAproductLine.BAobject.parameters.addAllRisksSpecified = true;

                GFRequest.BAproduct.BAproductLine.BAobject.parameters.post_code = FPRequest.customer.postnr.ToString();
                GFRequest.BAproduct.BAproductLine.BAobject.parameters.an_abandoned_agriculture = yes_no.N;
                GFRequest.BAproduct.BAproductLine.BAobject.parameters.retired_insurance = yes_no.N;
                GFRequest.BAproduct.BAproductLine.BAobject.parameters.bike_sum_code = 0;
                GFRequest.BAproduct.BAproductLine.BAobject.parameters.bike_sum_codeSpecified = true;
                GFRequest.BAproduct.BAproductLine.BAobject.parameters.sum_insured = 100;

                if (FPRequest.insurancerequest.indbo.alarm && FPRequest.insurancerequest.indbo.tilknyttet_alarmcentral)
                {
                    GFRequest.BAproduct.BAproductLine.BAobject.parameters.burglar_alarm = burglar_alarm_list.Item3;
                }
                else if (FPRequest.insurancerequest.indbo.alarm && !FPRequest.insurancerequest.indbo.tilknyttet_alarmcentral)
                {
                    GFRequest.BAproduct.BAproductLine.BAobject.parameters.burglar_alarm = burglar_alarm_list.Item2;
                }
                else
                {
                    GFRequest.BAproduct.BAproductLine.BAobject.parameters.burglar_alarm = burglar_alarm_list.Item1;
                }

                if (FPRequest.insurancerequest.indbo.selvrisiko > Convert.ToUInt16(selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].max_val))
                {
                    //If requested excess is bigger than GF's highest valid excess, set the general_excess to highest excess automatically without the rest of validation.
                    GFRequest.BAproduct.BAproductLine.BAobject.parameters.general_excess = excess_list.Item6;
                    newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].max_val);
                }
                else
                {
                    for (int i = selvrisko_res.tariff_code_record.Length; i > 0; i--)
                    {
                        if (FPRequest.insurancerequest.indbo.selvrisiko <= Convert.ToUInt16(selvrisko_res.tariff_code_record[i - 1].max_val))
                        {
                            switch (Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].code))
                            {
                                case 6: GFRequest.BAproduct.BAproductLine.BAobject.parameters.general_excess = excess_list.Item6; newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val); break;
                                case 5: GFRequest.BAproduct.BAproductLine.BAobject.parameters.general_excess = excess_list.Item5; newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val); break;
                                case 4: GFRequest.BAproduct.BAproductLine.BAobject.parameters.general_excess = excess_list.Item4; newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val); break;
                                case 3: GFRequest.BAproduct.BAproductLine.BAobject.parameters.general_excess = excess_list.Item3; newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val); break;
                                case 2: break; // Do nothing, it's only for retiree insurance excess.
                                case 1: GFRequest.BAproduct.BAproductLine.BAobject.parameters.general_excess = excess_list.Item1; newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val); break;
                                case 0: GFRequest.BAproduct.BAproductLine.BAobject.parameters.general_excess = excess_list.Item0; newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val); break;
                                default: break;
                            }                            
                        }
                    }
                }

                for (int i = indbo_sum_res.tariff_code_record.Length; i > 0; i--)
                {
                    if (FPRequest.insurancerequest.indbo.indbosum <= Convert.ToUInt32(indbo_sum_res.tariff_code_record[i - 1].max_val))
                    {
                        switch (Convert.ToInt32(indbo_sum_res.tariff_code_record[i - 1].code))
                        {
                            case 6: GFRequest.BAproduct.BAproductLine.BAobject.parameters.insurance_sum_a = sum_list_code.Item6; newIndbo = Convert.ToUInt32(indbo_sum_res.tariff_code_record[i - 1].max_val); break;
                            case 5: GFRequest.BAproduct.BAproductLine.BAobject.parameters.insurance_sum_a = sum_list_code.Item5; newIndbo = Convert.ToUInt32(indbo_sum_res.tariff_code_record[i - 1].max_val); break;
                            case 4: GFRequest.BAproduct.BAproductLine.BAobject.parameters.insurance_sum_a = sum_list_code.Item4; newIndbo = Convert.ToUInt32(indbo_sum_res.tariff_code_record[i - 1].max_val); break;
                            case 3: GFRequest.BAproduct.BAproductLine.BAobject.parameters.insurance_sum_a = sum_list_code.Item3; newIndbo = Convert.ToUInt32(indbo_sum_res.tariff_code_record[i - 1].max_val); break;
                            case 2: GFRequest.BAproduct.BAproductLine.BAobject.parameters.insurance_sum_a = sum_list_code.Item2; newIndbo = Convert.ToUInt32(indbo_sum_res.tariff_code_record[i - 1].max_val); break;
                            case 1: GFRequest.BAproduct.BAproductLine.BAobject.parameters.insurance_sum_a = sum_list_code.Item1; newIndbo = Convert.ToUInt32(indbo_sum_res.tariff_code_record[i - 1].max_val); break;
                            case 0: GFRequest.BAproduct.BAproductLine.BAobject.parameters.insurance_sum_a = sum_list_code.Item0; newIndbo = Convert.ToUInt32(indbo_sum_res.tariff_code_record[i - 1].max_val); break;
                        }
                    }

                    if (FPRequest.insurancerequest.indbo.indbosum > Convert.ToUInt32(indbo_sum_res.tariff_code_record[indbo_sum_res.tariff_code_record.Length - 1].max_val))
                    {
                        GFRequest.BAproduct.BAproductLine.BAobject.parameters.insurance_sum_b = (int)FPRequest.insurancerequest.indbo.indbosum;
                        GFRequest.BAproduct.BAproductLine.BAobject.parameters.insurance_sum_bSpecified = true;
                        GFRequest.BAproduct.BAproductLine.BAobject.parameters.insurance_sum_a = sum_list_code.Item6;                        
                        newIndbo = (int)FPRequest.insurancerequest.indbo.indbosum;                        
                        i = 0;
                    }
                }

                GFRequest.BAproduct.BAproductLine.BAobject.risk.indbo = new risk_generic();
                GFRequest.BAproduct.BAproductLine.BAobject.risk.indbo.risk_excess = newSelvrisko;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.indbo.risk_excessSpecified = true;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.indbo.risk_sum = (int)newIndbo;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.indbo.risk_sumSpecified = true;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.indbo.risk_yn = yes_no.Y;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.indbo.risk_ynSpecified = true;

                GFRequest.BAproduct.BAproductLine.BAobject.risk.privatansvar = new risk_generic();
                GFRequest.BAproduct.BAproductLine.BAobject.risk.privatansvar.risk_excess = newSelvrisko;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.privatansvar.risk_excessSpecified = true;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.privatansvar.risk_sum = (int)newIndbo;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.privatansvar.risk_sumSpecified = true;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.privatansvar.risk_yn = yes_no.Y;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.privatansvar.risk_ynSpecified = true;

                GFRequest.BAproduct.BAproductLine.BAobject.risk.elskade = new risk_generic();
                GFRequest.BAproduct.BAproductLine.BAobject.risk.elskade.risk_excess = newSelvrisko;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.elskade.risk_excessSpecified = true;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.elskade.risk_sum = (int)newIndbo;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.elskade.risk_sumSpecified = true;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.elskade.risk_yn = yes_no.Y;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.elskade.risk_ynSpecified = true;

                if (FPRequest.insurancerequest.indbo.elektronik)
                {

                    GFRequest.BAproduct.BAproductLine.BAobject.risk.elektronikforsikring = new risk_generic();
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.elektronikforsikring.risk_excess = 0;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.elektronikforsikring.risk_excessSpecified = true;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.elektronikforsikring.risk_sum = 0;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.elektronikforsikring.risk_sumSpecified = true;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.elektronikforsikring.risk_yn = yes_no.Y;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.elektronikforsikring.risk_ynSpecified = true;
                }

                if (FPRequest.insurancerequest.indbo.glaskumme)
                {
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.glas_og_kumme = new risk_generic();
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.glas_og_kumme.risk_excess = newSelvrisko;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.glas_og_kumme.risk_excessSpecified = true;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.glas_og_kumme.risk_sum = 0;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.glas_og_kumme.risk_sumSpecified = true;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.glas_og_kumme.risk_yn = yes_no.Y;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.glas_og_kumme.risk_ynSpecified = true;
                }

                if (FPRequest.insurancerequest.indbo.rejseeuropa)
                {
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__eu = new risk_generic();
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__eu.risk_excess = 0;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__eu.risk_excessSpecified = true;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__eu.risk_sum = 0;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__eu.risk_sumSpecified = true;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__eu.risk_yn = yes_no.Y;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__eu.risk_ynSpecified = true;
                }

                if (FPRequest.insurancerequest.indbo.rejseverden)
                {
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__verden = new risk_generic();
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__verden.risk_excess = 0;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__verden.risk_excessSpecified = true;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__verden.risk_sum = 0;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__verden.risk_sumSpecified = true;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__verden.risk_yn = yes_no.Y;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__verden.risk_ynSpecified = true;
                }

                if (FPRequest.insurancerequest.indbo.afbestillingsforsikring)
                {
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__afbestilling = new risk_generic();
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__afbestilling.risk_excess = 0;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__afbestilling.risk_excessSpecified = true;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__afbestilling.risk_sum = 0;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__afbestilling.risk_sumSpecified = true;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__afbestilling.risk_yn = yes_no.Y;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__afbestilling.risk_ynSpecified = true;
                }

                if (FPRequest.insurancerequest.indbo.pludseligskade)
                {
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.pludselig_skade = new risk_generic();
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.pludselig_skade.risk_excess = 0;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.pludselig_skade.risk_excessSpecified = true;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.pludselig_skade.risk_sum = 0;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.pludselig_skade.risk_sumSpecified = true;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.pludselig_skade.risk_yn = yes_no.Y;
                    GFRequest.BAproduct.BAproductLine.BAobject.risk.pludselig_skade.risk_ynSpecified = true;
                }

                GFRequest.BAproduct.BAproductLine.BAobject.risk.udvidet_guld_og_soelv = new risk_generic();
                GFRequest.BAproduct.BAproductLine.BAobject.risk.udvidet_guld_og_soelv.risk_excess = newSelvrisko;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.udvidet_guld_og_soelv.risk_excessSpecified = true;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.udvidet_guld_og_soelv.risk_sum = 0;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.udvidet_guld_og_soelv.risk_sumSpecified = false;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.udvidet_guld_og_soelv.risk_yn = yes_no.N;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.udvidet_guld_og_soelv.risk_ynSpecified = true;

                GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__ski = new risk_generic();
                GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__ski.risk_excess = 0;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__ski.risk_excessSpecified = true;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__ski.risk_sum = 0;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__ski.risk_sumSpecified = true;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__ski.risk_yn = yes_no.N;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.rejse__ski.risk_ynSpecified = true;

                GFRequest.BAproduct.BAproductLine.BAobject.risk.smaabaade_og_windsurfer = new risk_generic();
                GFRequest.BAproduct.BAproductLine.BAobject.risk.smaabaade_og_windsurfer.risk_excess = newSelvrisko;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.smaabaade_og_windsurfer.risk_excessSpecified = true;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.smaabaade_og_windsurfer.risk_sum = 0;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.smaabaade_og_windsurfer.risk_sumSpecified = false;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.smaabaade_og_windsurfer.risk_yn = yes_no.N;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.smaabaade_og_windsurfer.risk_ynSpecified = true;

                GFRequest.BAproduct.BAproductLine.BAobject.risk.udvidet_cykeldaekning = new risk_generic();
                GFRequest.BAproduct.BAproductLine.BAobject.risk.udvidet_cykeldaekning.risk_excess = 0;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.udvidet_cykeldaekning.risk_excessSpecified = false;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.udvidet_cykeldaekning.risk_sum = 0;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.udvidet_cykeldaekning.risk_sumSpecified = false;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.udvidet_cykeldaekning.risk_yn = yes_no.N;
                GFRequest.BAproduct.BAproductLine.BAobject.risk.udvidet_cykeldaekning.risk_ynSpecified = false;
            }
            catch (Exception exc)
            {
                ErrorsLogger.Error(new ErrorInfo(exc.ToString()));
                GFRequest = null;
                return false;
            }
            return true;
        }

        public static bool ConvertHusRequest(request FPRequest, out HU_request GFRequest, out string error)
        {         
            int newSelvrisko = 0;
            error = null;

            try
            {
                TIAReferenceServiceClient client = new TIAReferenceServiceClient();

                tariff_codes_request selvrisko_req = new tariff_codes_request();
                tariff_codes_response selvrisko_res = new tariff_codes_response();

                selvrisko_req.product_line_id = "HU";
                selvrisko_req.table_name = "SELVRISIKO";
                selvrisko_req.order_by = "MAX_VALUE";
                selvrisko_req.indexed = "Y";
                if (!client.GetTariffCode(selvrisko_req, out selvrisko_res, out error))
                {
                    GFRequest = null;
                    return false;
                }
                else if (selvrisko_res.error_record != null)
                {
                    error = selvrisko_res.error_record.error_description;
                    GFRequest = null;
                    return false;
                }

                GFRequest = new HU_request();
                GFRequest.HUproduct = new HUproduct();
                GFRequest.HUproduct.HUproductLine = new HUproductLine();
                GFRequest.HUproduct.HUproductLine.HUobject = new HUobject();
                GFRequest.HUproduct.HUproductLine.HUobject.parameters = new parameters_hu();
                GFRequest.HUproduct.HUproductLine.HUobject.risk = new risk_hu();

                GFRequest.HUproduct.HUproductLine.affinity = FPRequest.affinity_no;
                GFRequest.HUproduct.HUproductLine.affinitySpecified = true;

                GFRequest.HUproduct.HUproductLine.HUobject.parameters.const_year = FPRequest.insurancerequest.hus.opfoerelsesaar.ToString();
                // GFRequest.HUproduct.HUproductLine.HUobject.parameters.ar_living_qrtr = FPRequest.insurancerequest.hus.bolig_m2; artf239542
                GFRequest.HUproduct.HUproductLine.HUobject.parameters.ar_living_qrtr = FPRequest.insurancerequest.hus.bebygget_m2;
                GFRequest.HUproduct.HUproductLine.HUobject.parameters.ar_outhouse = FPRequest.insurancerequest.hus.garage_m2;

                if ((int)FPRequest.insurancerequest.hus.etager > 1)
                {
                    GFRequest.HUproduct.HUproductLine.HUobject.parameters.no_of_floors = no_of_floors.Item2;
                }
                else
                {
                    GFRequest.HUproduct.HUproductLine.HUobject.parameters.no_of_floors = no_of_floors.Item1;
                }

                if ((int)FPRequest.insurancerequest.hus.kaelder_m2 > 0)
                {
                    GFRequest.HUproduct.HUproductLine.HUobject.parameters.basement = yes_no2.J;
                }
                else
                {
                    GFRequest.HUproduct.HUproductLine.HUobject.parameters.basement = yes_no2.N;
                }

                GFRequest.HUproduct.HUproductLine.HUobject.parameters.post_area = FPRequest.customer.postnr.ToString();

                if (FPRequest.insurancerequest.hus.vandstop)
                {
                    GFRequest.HUproduct.HUproductLine.HUobject.parameters.water_stop = yes_no.Y;
                }
                else
                {
                    GFRequest.HUproduct.HUproductLine.HUobject.parameters.water_stop = yes_no.N;
                }

                if (FPRequest.insurancerequest.hus.nedlagt_landbrug)
                {
                    GFRequest.HUproduct.HUproductLine.HUobject.parameters.an_abandoned_agriculture = yes_no.Y;
                }
                else
                {
                    GFRequest.HUproduct.HUproductLine.HUobject.parameters.an_abandoned_agriculture = yes_no.N;
                }

                GFRequest.HUproduct.HUproductLine.HUobject.parameters.general_excess = FPRequest.insurancerequest.hus.selvrisiko;

                switch (FPRequest.insurancerequest.hus.opvarmning)
                {
                    //We are only interesting in items 6 & 9 because those two are not acceptable by GF
                    //case HeatingCodeType.Item1: break;
                    //case HeatingCodeType.Item2: break;
                    //case HeatingCodeType.Item3: break;
                    //case HeatingCodeType.Item4: break;
                    case HeatingCodeType.Item6: GFRequest.HUproduct.HUproductLine.HUobject.parameters.standard_heating = yes_no2.N; GFRequest.HUproduct.HUproductLine.HUobject.parameters.other_heating = heating.Item4; break;
                    //case HeatingCodeType.Item7: break;
                    case HeatingCodeType.Item9: GFRequest.HUproduct.HUproductLine.HUobject.parameters.standard_heating = yes_no2.N; GFRequest.HUproduct.HUproductLine.HUobject.parameters.other_heating = heating.Item4; break;
                    //case HeatingCodeType.Item51: break;
                    //case HeatingCodeType.Item55: break;
                    default: GFRequest.HUproduct.HUproductLine.HUobject.parameters.standard_heating = yes_no2.J; GFRequest.HUproduct.HUproductLine.HUobject.parameters.other_heating = heating.Item1; break;
                }                
                
                switch (FPRequest.insurancerequest.hus.tagbeklaedning)
                {
                    //We are only interesting in item 7 because this one is not acceptable by GF
                    //case RoofingMaterialCodeType.Item1: break;
                    //case RoofingMaterialCodeType.Item2: break;
                    //case RoofingMaterialCodeType.Item3: break;
                    //case RoofingMaterialCodeType.Item4: break;
                    //case RoofingMaterialCodeType.Item5: break;
                    //case RoofingMaterialCodeType.Item6: break;
                    case RoofingMaterialCodeType.Item7: GFRequest.HUproduct.HUproductLine.HUobject.parameters.roof_type = roof_type.Item3; break;
                    //case RoofingMaterialCodeType.Item10: break;
                    //case RoofingMaterialCodeType.Item11: break;
                    //case RoofingMaterialCodeType.Item12: break;
                    //case RoofingMaterialCodeType.Item20: break;
                    //case RoofingMaterialCodeType.Item80: break;
                    //case RoofingMaterialCodeType.Item90: break;
                    default: GFRequest.HUproduct.HUproductLine.HUobject.parameters.roof_type = roof_type.Item1; break;
                }

                switch (FPRequest.insurancerequest.hus.ydervaeg_kode)
                {
                    case OuterWallsMaterialCodeType.Item1: GFRequest.HUproduct.HUproductLine.HUobject.parameters.primary_construction_type = construction_type.Item4; break;
                    case OuterWallsMaterialCodeType.Item2: GFRequest.HUproduct.HUproductLine.HUobject.parameters.primary_construction_type = construction_type.Item4; break;
                    case OuterWallsMaterialCodeType.Item3: GFRequest.HUproduct.HUproductLine.HUobject.parameters.primary_construction_type = construction_type.Item4; break;
                    case OuterWallsMaterialCodeType.Item4: GFRequest.HUproduct.HUproductLine.HUobject.parameters.primary_construction_type = construction_type.Item3; break;
                    case OuterWallsMaterialCodeType.Item5: GFRequest.HUproduct.HUproductLine.HUobject.parameters.primary_construction_type = construction_type.Item1; break;
                    case OuterWallsMaterialCodeType.Item6: GFRequest.HUproduct.HUproductLine.HUobject.parameters.primary_construction_type = construction_type.Item4; break;
                    //case OuterWallsMaterialCodeType.Item8: break;
                    case OuterWallsMaterialCodeType.Item10: GFRequest.HUproduct.HUproductLine.HUobject.parameters.primary_construction_type = construction_type.Item4; break;
                    //case OuterWallsMaterialCodeType.Item11: break;
                    //case OuterWallsMaterialCodeType.Item12: break;
                    //case OuterWallsMaterialCodeType.Item80: break;
                    //case OuterWallsMaterialCodeType.Item90: break;
                    default: break;
                }
                

                // Values ​​that do not affect the price of insurance
                GFRequest.HUproduct.HUproductLine.HUobject.parameters.ejerskifte = yes_no.Y;
                GFRequest.HUproduct.HUproductLine.HUobject.parameters.raadskema_ok = yes_no.Y;
                GFRequest.HUproduct.HUproductLine.HUobject.parameters.is_preserved = yes_no.N;
                GFRequest.HUproduct.HUproductLine.HUobject.parameters.is_on_leased = is_on_leased.Item1;
                GFRequest.HUproduct.HUproductLine.HUobject.parameters.is_econ_activity = yes_no.N;
                GFRequest.HUproduct.HUproductLine.HUobject.parameters.is_build_cons_recons_ext = is_build_cons_recons_ext.Item1;

                if (FPRequest.insurancerequest.hus.selvrisiko > Convert.ToUInt16(selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length-1].max_val))
                {
                    //If requested excess is bigger than GF's highest valid excess, set the general_excess to highest excess automatically without the rest of validation.
                    newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].max_val);
                    GFRequest.HUproduct.HUproductLine.HUobject.parameters.general_excess = Convert.ToInt32(selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].code);                    
                }
                else
                {

                    /*for (int i = selvrisko_res.tariff_code_record.Length; i > 0; i--)
                    {
                        if (FPRequest.insurancerequest.hus.selvrisiko <= Convert.ToUInt16(selvrisko_res.tariff_code_record[i - 1].max_val))
                        {
                            newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val);
                            GFRequest.HUproduct.HUproductLine.HUobject.parameters.general_excess = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].code);
                        }
                    }*/
                    for (int i = 0; i < selvrisko_res.tariff_code_record.Length; i++)
                    {
                        if (FPRequest.insurancerequest.hus.selvrisiko >= Convert.ToUInt16(selvrisko_res.tariff_code_record[i].max_val) && FPRequest.insurancerequest.hus.selvrisiko <= Convert.ToUInt16(selvrisko_res.tariff_code_record[i + 1].max_val))
                        {
                            if (Math.Abs(FPRequest.insurancerequest.hus.selvrisiko - Convert.ToUInt16(selvrisko_res.tariff_code_record[i].max_val)) < Math.Abs(FPRequest.insurancerequest.hus.selvrisiko - Convert.ToUInt16(selvrisko_res.tariff_code_record[i + 1].max_val)))
                            {
                                newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i].max_val);
                            }
                            else
                            {
                                newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i + 1].max_val);
                            }
                            GFRequest.HUproduct.HUproductLine.HUobject.parameters.general_excess = newSelvrisko;
                            break;
                        }
                    }
                }

                GFRequest.HUproduct.HUproductLine.HUobject.risk.bygningsbrand = new risk_generic();
                GFRequest.HUproduct.HUproductLine.HUobject.risk.bygningsbrand.risk_excess = newSelvrisko;
                GFRequest.HUproduct.HUproductLine.HUobject.risk.bygningsbrand.risk_excessSpecified = true;
                GFRequest.HUproduct.HUproductLine.HUobject.risk.bygningsbrand.risk_sum = 0;// 1584641;
                GFRequest.HUproduct.HUproductLine.HUobject.risk.bygningsbrand.risk_sumSpecified = true;
                GFRequest.HUproduct.HUproductLine.HUobject.risk.bygningsbrand.risk_yn = yes_no.Y;
                GFRequest.HUproduct.HUproductLine.HUobject.risk.bygningsbrand.risk_ynSpecified = true;

                GFRequest.HUproduct.HUproductLine.HUobject.risk.bygningskasko = new risk_generic();
                GFRequest.HUproduct.HUproductLine.HUobject.risk.bygningskasko.risk_excess = newSelvrisko;
                GFRequest.HUproduct.HUproductLine.HUobject.risk.bygningskasko.risk_excessSpecified = true;
                GFRequest.HUproduct.HUproductLine.HUobject.risk.bygningskasko.risk_yn = yes_no.Y;
                GFRequest.HUproduct.HUproductLine.HUobject.risk.bygningskasko.risk_ynSpecified = true;

                if (FPRequest.insurancerequest.hus.svampinsekt)
                {
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.svamp_insekt_og_raad = new risk_generic();
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.svamp_insekt_og_raad.risk_excess = newSelvrisko;
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.svamp_insekt_og_raad.risk_excessSpecified = true;
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.svamp_insekt_og_raad.risk_yn = yes_no.Y;
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.svamp_insekt_og_raad.risk_ynSpecified = true;                                        
                }
                if (FPRequest.insurancerequest.hus.raad)
                {
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.udvidet_raad = new risk_generic();
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.udvidet_raad.risk_excess = newSelvrisko;
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.udvidet_raad.risk_excessSpecified = true;
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.udvidet_raad.risk_yn = yes_no.Y;
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.udvidet_raad.risk_ynSpecified = true;
                }
                if (FPRequest.insurancerequest.hus.roerkablerstikledninger)
                {
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.udvidet_roerskade = new risk_generic();
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.udvidet_roerskade.risk_excess = newSelvrisko;
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.udvidet_roerskade.risk_excessSpecified = true;
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.udvidet_roerskade.risk_yn = yes_no.Y;
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.udvidet_roerskade.risk_ynSpecified = true;
                }
                if (FPRequest.insurancerequest.hus.dyrskader)
                {
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.udvidet_skadedyr = new risk_generic();
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.udvidet_skadedyr.risk_excess = newSelvrisko;
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.udvidet_skadedyr.risk_excessSpecified = true;
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.udvidet_skadedyr.risk_yn = yes_no.Y;
                    GFRequest.HUproduct.HUproductLine.HUobject.risk.udvidet_skadedyr.risk_ynSpecified = true;
                }
            }
            catch (Exception exc)
            {
                ErrorsLogger.Error(new ErrorInfo(exc.ToString()));                
                GFRequest = null;
                return false;
            }

            return true;
        }

        public static bool ConvertMicroHusRequest(request FPRequest, out HS_request GFRequest, out string error)
        {
            int newSelvrisko = 0;
            error = null;

            try
            {
                GFRequest = new HS_request();
                GFRequest.HSproduct = new HSproduct();
                GFRequest.HSproduct.HSproductLine = new HSproductLine();
                GFRequest.HSproduct.HSproductLine.HSobject = new HSobject();
                GFRequest.HSproduct.HSproductLine.HSobject.parameters = new parameters_hs();
                GFRequest.HSproduct.HSproductLine.HSobject.risk = new risk_hs();

                GFRequest.HSproduct.HSproductLine.affinity = FPRequest.affinity_no;
                GFRequest.HSproduct.HSproductLine.affinitySpecified = true;

                #region Call to extern WS
                TIAReferenceServiceClient client = new TIAReferenceServiceClient();

                tariff_codes_request indbo_sum_req = new tariff_codes_request();
                tariff_codes_response indbo_sum_res = new tariff_codes_response();

                tariff_codes_request selvrisko_req = new tariff_codes_request();
                tariff_codes_response selvrisko_res = new tariff_codes_response();

                BbrDataResponse bbrResponse;

                indbo_sum_req.product_line_id = "HS";
                indbo_sum_req.table_name = "SUM_POL";
                indbo_sum_req.order_by = "MAX_VALUE";
                indbo_sum_req.indexed = "Y";
                if (!client.GetTariffCode(indbo_sum_req, out indbo_sum_res, out error))
                {
                    GFRequest = null;
                    return false;
                }
                else if (indbo_sum_res.error_record != null)
                {
                    error = indbo_sum_res.error_record.error_description;
                    GFRequest = null;
                    return false;
                }

                selvrisko_req.product_line_id = "HS";
                selvrisko_req.table_name = "SELV_GENERE";
                selvrisko_req.order_by = "MAX_VALUE";
                selvrisko_req.indexed = "Y";
                if (!client.GetTariffCode(selvrisko_req, out selvrisko_res, out error))
                {
                    GFRequest = null;
                    return false;
                }
                else if (selvrisko_res.error_record != null)
                {
                    error = selvrisko_res.error_record.error_description;
                    GFRequest = null;
                    return false;
                }
                // KKK VVVV HHHH EE SSSS
                // 461 7258 073C st 00th
                // 461 7258 073C ST TH
                // 461 3502 019

                string newKVHX;
                bbrResponse = GetBBRData(FPRequest.customer.kvhx, out newKVHX);

                if (bbrResponse.BbrDataEntries.Length == 0)
                {
                    SendMail(FPRequest.customer.kvhx.ToUpper(), FPRequest.customer.adresse);
                    error = "KVHX Index not correct";
                    GFRequest = null;
                    return false;
                }
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_kvhx = newKVHX;
                #endregion Call to extern WS

                #region parameters
                #region default parameters
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.rki_yn = "N";
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.no_of_cloudburst_damages = 0;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_use_of_building = "PRIVAT";
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.ongoing_building = "NEJ";
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.ongoing_building_end_date = null;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.last_get_date_bbr = null;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.inspection_date = null;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.house_wind_turbine_yn = "N";
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.firealarm_yn = "N";
                #endregion default parameters

                #region custom parameters
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.damages_last_years = (short)FPRequest.insurancerequest.hus.antal_skader_treaar;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_buildyear = FPRequest.insurancerequest.hus.opfoerelsesaar.ToString();
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_buildyear = FPRequest.insurancerequest.hus.opfoerelsesaar.ToString();
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_livingspace_total_sqm = FPRequest.insurancerequest.hus.bolig_m2;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_livingspace_total_sqm = (int)bbrResponse.BbrDataEntries[0].BeboetAreal;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_no_of_floors = (int)FPRequest.insurancerequest.hus.etager;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_no_of_floors = (int)bbrResponse.BbrDataEntries[0].AntalEtagerIBygning;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_basement_sqm = (int)FPRequest.insurancerequest.hus.kaelder_m2;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_basement_sqm = (int)bbrResponse.BbrDataEntries[0].Kaelderareal;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_no_of_toilets = FPRequest.insurancerequest.hus.toiletbadrum.ToString();
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_outh_sqm = (int)bbrResponse.BbrDataEntries[0].Udestueareal;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_smallbuild_sqm = (int)bbrResponse.BbrDataEntries[0].ArealAfSmaabygninger;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_samlet_bebygget_areal = (int)bbrResponse.BbrDataEntries[0].SamletBebyggetAreal;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_samlet_boligareal_i_byg = (int)bbrResponse.BbrDataEntries[0].SamletBoligarealIBygning;

                switch (FPRequest.insurancerequest.hus.tagbeklaedning)
                {
                    case RoofingMaterialCodeType.Item1: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rooftype = "BUILT_UP"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_rooftype = "1"; break; //1: Built-up
                    case RoofingMaterialCodeType.Item2: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rooftype = "BUILT_UP"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_rooftype = "2"; break; //2: Tagpap (med taghældning)
                    case RoofingMaterialCodeType.Item3: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rooftype = "HARD"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_rooftype = "3"; break; //3: Fibercement, herunder asbest (bølge- eller skifer-eternit)
                    case RoofingMaterialCodeType.Item4: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rooftype = "HARD"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_rooftype = "4"; break; //4: Cementsten
                    case RoofingMaterialCodeType.Item5: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rooftype = "HARD"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_rooftype = "5"; break; //5: Tegl
                    case RoofingMaterialCodeType.Item6: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rooftype = "HARD"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_rooftype = "6"; break; //6: Metalplader (bolgeblik, aluminium, o.lign.)
                    case RoofingMaterialCodeType.Item7: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rooftype = "STRA"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_rooftype = "7"; break; //7: Stråtag
                    case RoofingMaterialCodeType.Item10: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rooftype = "HARD"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_rooftype = "10"; break; //10: Fibercement (asbestfri)
                    case RoofingMaterialCodeType.Item11: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rooftype = "PVC"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_rooftype = "11"; break; //11: PVC
                    case RoofingMaterialCodeType.Item12: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rooftype = "GLAS"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_rooftype = "12"; break; //12: Glas                    
                    case RoofingMaterialCodeType.Item80: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rooftype = null; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_rooftype = "80"; break; //80: Ingen (Tages ud af listen, man kan vælge blandt)
                    case RoofingMaterialCodeType.Item90: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rooftype = null; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_rooftype = "90"; break; //90: Andet materiale
                    default: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rooftype = null; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_rooftype = null; break; //Unknown
                }

                switch (FPRequest.insurancerequest.hus.ydervaeg_kode)
                {
                    case OuterWallsMaterialCodeType.Item1: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_exteriorwall_material = "MUR"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_exteriorwall_material = "1"; break; //Mursten (tegl, kalksten, cementsten)
                    case OuterWallsMaterialCodeType.Item2: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_exteriorwall_material = "MUR"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_exteriorwall_material = "2"; break; //Letbeton (lette bloksten, gasbeton)
                    case OuterWallsMaterialCodeType.Item3: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_exteriorwall_material = "MUR"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_exteriorwall_material = "3"; break; //Plader af fibercement, herunder asbest (eternit el. lign.)
                    case OuterWallsMaterialCodeType.Item4: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_exteriorwall_material = "BINDING"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_exteriorwall_material = "4"; break; //Bindingsværk (med udvendigt synligt træværk)
                    case OuterWallsMaterialCodeType.Item5: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_exteriorwall_material = null; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_exteriorwall_material = "5"; break; //Træbeklædning
                    case OuterWallsMaterialCodeType.Item6: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_exteriorwall_material = "MUR"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_exteriorwall_material = "6"; break; //Betonelementer (bygninghøje betonelementer)
                    case OuterWallsMaterialCodeType.Item8: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_exteriorwall_material = "MUR"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_exteriorwall_material = "8"; break; //Metalplader
                    case OuterWallsMaterialCodeType.Item10: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_exteriorwall_material = "MUR"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_exteriorwall_material = "10"; break; //Plader af fibercement (asbestfri)
                    case OuterWallsMaterialCodeType.Item11: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_exteriorwall_material = "GLAS_PVC"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_exteriorwall_material = "11"; break; //PVC
                    case OuterWallsMaterialCodeType.Item12: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_exteriorwall_material = "GLAS_PVC"; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_exteriorwall_material = "12"; break; //Glas
                    case OuterWallsMaterialCodeType.Item80: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_exteriorwall_material = null; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_exteriorwall_material = "80"; break; //Ingen
                    case OuterWallsMaterialCodeType.Item90: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_exteriorwall_material = null; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_exteriorwall_material = "90"; break; //Andet
                    default: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_exteriorwall_material = null; GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_exteriorwall_material = null; break; //Unknown
                }

                switch (FPRequest.insurancerequest.hus.opvarmning)
                {
                    case HeatingCodeType.Item1: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_primary_warming = "EL_LUFT_LU"; break; //Elektricitet
                    case HeatingCodeType.Item2: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_primary_warming = "FJERNVARME"; break; //Gasvarksgas
                    case HeatingCodeType.Item3: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_primary_warming = "FJERNVARME"; break; //Flydende brandsel (olie, petroleum, flaskegas)
                    case HeatingCodeType.Item4: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_primary_warming = "FAST_BRAEN"; break; //Fast brandsel (kul, koks, brande mm.)                    
                    case HeatingCodeType.Item6: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_primary_warming = "HALMFYR"; break; //Halm
                    case HeatingCodeType.Item7: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_primary_warming = "FJERNVARME"; break; //Naturgas
                    case HeatingCodeType.Item51: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_primary_warming = "FJERNVARME"; break; // Fjernvarme *)
                    case HeatingCodeType.Item55: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_primary_warming = "EL_LUFT_VA"; break; //Varmepumpe *)
                    case HeatingCodeType.Item9: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_primary_warming = "HALMFYR"; break; //Andet
                    default: break;
                }

                switch (bbrResponse.BbrDataEntries[0].SupplerendeVarmekildeKode)
                {
                    /*
01	Varmepumpeanlæg
02	Ovne til fast brændsel (brændeovn o. lign.)
03	Ovne til flydende brændsel
04	Solpaneler
05	Pejs
06	Gasradiator
07	Elovne, elpaneler
10	Biogasanlæg
80	Andet
90	Bygningen har ingen supplerende varme
                */

                    //case "01": break;
                    //case "02": break;
                    //case "03": break;
                    //case "04": break;
                    //case "05": break;
                    //case "06": break;
                    //case "07": break;
                    //case "10": break;
                    //case "80": break;
                    //case "90": break;
                    default: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_additional_energy_source = "INGEN"; break;
                }

                GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_outhouse_total_sqm = FPRequest.insurancerequest.hus.garage_m2;
                //GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_outh_sqm = FPRequest.insurancerequest.hus.garage_m2;

                if (bbrResponse.BbrDataEntries[0].Udlejningsforhold == "Benyttet af ejeren")
                {
                    GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rental = "NEJ";
                }
                else
                {
                    if (bbrResponse.BbrDataEntries[0].AntalBeboelseslejligheder > 5)
                    {
                        GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rental = "UDLEJ05";
                    }
                    else
                    {
                        GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_rental = "UDLEJ0" + bbrResponse.BbrDataEntries[0].AntalBeboelseslejligheder;
                    }
                }

                GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_ant_erhvenh_paa_ejd = FPRequest.insurancerequest.hus.erhverv_m2;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_business_sqm = (int)bbrResponse.BbrDataEntries[0].Erhvervsareal;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_total_business_sqm = (int)bbrResponse.BbrDataEntries[0].Erhvervsareal;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.business_type = "INGEN";
                switch (bbrResponse.BbrDataEntries[0].Fredningsforhold)
                {
                    case "01": GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_preservationconditions = "FREDET"; break;
                    case "02": GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_preservationconditions = "FREDET"; break;
                    case "03": GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_preservationconditions = "BEVARINGSV"; break;
                    case "04": GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_preservationconditions = "FREDET"; break;
                    case "05": GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_preservationconditions = "BEVARINGSV"; break;
                    case "06": GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_preservationconditions = "FREDET"; break;
                    case "07": GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_preservationconditions = "FREDET"; break;
                    case "08": GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_preservationconditions = "BEVARINGSV"; break;
                    case "09": GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_preservationconditions = "BEVARINGSV"; break;
                    default: GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_preservationconditions = "INGEN"; break;
                }

                if (bbrResponse.BbrDataEntries[0].SupplerendeVarmekildeKode == "04")
                {
                    GFRequest.HSproduct.HSproductLine.HSobject.parameters.solarcells_yn = "Y";
                }
                else
                {
                    GFRequest.HSproduct.HSproductLine.HSobject.parameters.solarcells_yn = "N";
                }

                if (FPRequest.insurancerequest.hus.hoejvandslukke)
                {
                    GFRequest.HSproduct.HSproductLine.HSobject.parameters.backflowblocker_yn = "Y";
                }
                else
                {
                    GFRequest.HSproduct.HSproductLine.HSobject.parameters.backflowblocker_yn = "N";
                }

                if (FPRequest.insurancerequest.hus.vandstop)
                {
                    GFRequest.HSproduct.HSproductLine.HSobject.parameters.wateralarm_yn = "Y";
                }
                else
                {
                    GFRequest.HSproduct.HSproductLine.HSobject.parameters.wateralarm_yn = "N";
                }

                GFRequest.HSproduct.HSproductLine.HSobject.parameters.tariff_user_age = (short)FPRequest.customer.alder;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.tariff_user_age_at_first_start = (short)FPRequest.customer.alder;
                GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_zone = bbrResponse.BbrDataEntries[0].ZonekodeKode;

                if (GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_zone == "1")
                {
                    GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_zone = "BY";
                }
                else if (GFRequest.HSproduct.HSproductLine.HSobject.parameters.bbr_zone == "2")
                {
                    GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_zone = "LAND";
                }
                else
                {
                    GFRequest.HSproduct.HSproductLine.HSobject.parameters.gf_zone = "OVRIGE";
                }

                if (FPRequest.insurancerequest.hus.selvrisiko > Convert.ToUInt16(selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].max_val))
                {
                    //If requested excess is bigger than GF's highest valid excess, set the general_excess to highest excess automatically without the rest of validation.
                    newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].max_val);
                    GFRequest.HSproduct.HSproductLine.HSobject.parameters.general_excess_code = selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].code;
                    GFRequest.HSproduct.HSproductLine.HSobject.parameters.electricity_excess_code = selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].code;
                    GFRequest.HSproduct.HSproductLine.HSobject.parameters.glass_excess_code = selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].code;
                }
                else
                {

                    /*for (int i = selvrisko_res.tariff_code_record.Length; i > 0; i--)
                    {
                        if (FPRequest.insurancerequest.hus.selvrisiko <= Convert.ToUInt16(selvrisko_res.tariff_code_record[i - 1].max_val))
                        {
                            newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val);
                            GFRequest.HSproduct.HSproductLine.HSobject.parameters.general_excess_code = selvrisko_res.tariff_code_record[i - 1].code;
                            GFRequest.HSproduct.HSproductLine.HSobject.parameters.electricity_excess_code = selvrisko_res.tariff_code_record[i - 1].code;
                            GFRequest.HSproduct.HSproductLine.HSobject.parameters.glass_excess_code = selvrisko_res.tariff_code_record[i - 1].code;
                        }
                    }*/
                    for (int i = 0; i < selvrisko_res.tariff_code_record.Length; i++)
                    {
                        if (FPRequest.insurancerequest.hus.selvrisiko >= Convert.ToUInt16(selvrisko_res.tariff_code_record[i].max_val) && FPRequest.insurancerequest.hus.selvrisiko <= Convert.ToUInt16(selvrisko_res.tariff_code_record[i + 1].max_val))
                        {
                            if (Math.Abs(FPRequest.insurancerequest.hus.selvrisiko - Convert.ToUInt16(selvrisko_res.tariff_code_record[i].max_val)) < Math.Abs(FPRequest.insurancerequest.hus.selvrisiko - Convert.ToUInt16(selvrisko_res.tariff_code_record[i + 1].max_val)))
                            {
                                newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i].max_val);
                                GFRequest.HSproduct.HSproductLine.HSobject.parameters.general_excess_code = selvrisko_res.tariff_code_record[i].code;
                                GFRequest.HSproduct.HSproductLine.HSobject.parameters.electricity_excess_code = selvrisko_res.tariff_code_record[i].code;
                                GFRequest.HSproduct.HSproductLine.HSobject.parameters.glass_excess_code = selvrisko_res.tariff_code_record[i].code;
                            }
                            else
                            {
                                newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i + 1].max_val);
                                GFRequest.HSproduct.HSproductLine.HSobject.parameters.general_excess_code = selvrisko_res.tariff_code_record[i + 1].code;
                                GFRequest.HSproduct.HSproductLine.HSobject.parameters.electricity_excess_code = selvrisko_res.tariff_code_record[i + 1].code;
                                GFRequest.HSproduct.HSproductLine.HSobject.parameters.glass_excess_code = selvrisko_res.tariff_code_record[i + 1].code;
                            }                            
                            break;
                        }
                    }

                }
                #endregion custom parameters
                #endregion parameters

                #region risks
                #region mandatory risks
                // Brand (17) 
                GFRequest.HSproduct.HSproductLine.HSobject.risk.brand = new risk_generic();
                GFRequest.HSproduct.HSproductLine.HSobject.risk.brand.risk_excess = newSelvrisko;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.brand.risk_excessSpecified = true;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.brand.risk_yn = yes_no.Y;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.brand.risk_ynSpecified = true;

                // Ildsvåde, lyn, eksplosion m.m. (1) 
                GFRequest.HSproduct.HSproductLine.HSobject.risk.ildsvaade_lyn_eksplosion_mm = new risk_generic();
                GFRequest.HSproduct.HSproductLine.HSobject.risk.ildsvaade_lyn_eksplosion_mm.risk_excess = newSelvrisko;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.ildsvaade_lyn_eksplosion_mm.risk_excessSpecified = true;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.ildsvaade_lyn_eksplosion_mm.risk_yn = yes_no.Y;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.ildsvaade_lyn_eksplosion_mm.risk_ynSpecified = true;

                // Elskade (7) 
                GFRequest.HSproduct.HSproductLine.HSobject.risk.elskade = new risk_generic();
                GFRequest.HSproduct.HSproductLine.HSobject.risk.elskade.risk_excess = newSelvrisko;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.elskade.risk_excessSpecified = true;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.elskade.risk_yn = yes_no.Y;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.elskade.risk_ynSpecified = true;

                // Kasko (2) 
                GFRequest.HSproduct.HSproductLine.HSobject.risk.kasko = new risk_generic();
                GFRequest.HSproduct.HSproductLine.HSobject.risk.kasko.risk_excess = newSelvrisko;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.kasko.risk_excessSpecified = true;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.kasko.risk_yn = yes_no.Y;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.kasko.risk_ynSpecified = true;

                // Vejr og vand (8) 
                GFRequest.HSproduct.HSproductLine.HSobject.risk.vejr_og_vand = new risk_generic();
                GFRequest.HSproduct.HSproductLine.HSobject.risk.vejr_og_vand.risk_excess = newSelvrisko;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.vejr_og_vand.risk_excessSpecified = true;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.vejr_og_vand.risk_yn = yes_no.Y;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.vejr_og_vand.risk_ynSpecified = true;

                // Tyveri og hærværk (9) 
                GFRequest.HSproduct.HSproductLine.HSobject.risk.tyveri_og_haervaerk = new risk_generic();
                GFRequest.HSproduct.HSproductLine.HSobject.risk.tyveri_og_haervaerk.risk_excess = newSelvrisko;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.tyveri_og_haervaerk.risk_excessSpecified = true;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.tyveri_og_haervaerk.risk_yn = yes_no.Y;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.tyveri_og_haervaerk.risk_ynSpecified = true;

                // Pludselig skade (10) 
                GFRequest.HSproduct.HSproductLine.HSobject.risk.pludselig_skade = new risk_generic();
                GFRequest.HSproduct.HSproductLine.HSobject.risk.pludselig_skade.risk_excess = newSelvrisko;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.pludselig_skade.risk_excessSpecified = true;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.pludselig_skade.risk_yn = yes_no.Y;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.pludselig_skade.risk_ynSpecified = true;

                // Glas og sanitet (11) 
                GFRequest.HSproduct.HSproductLine.HSobject.risk.glas_og_sanitet = new risk_generic();
                GFRequest.HSproduct.HSproductLine.HSobject.risk.glas_og_sanitet.risk_excess = newSelvrisko;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.glas_og_sanitet.risk_excessSpecified = true;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.glas_og_sanitet.risk_yn = yes_no.Y;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.glas_og_sanitet.risk_ynSpecified = true;

                // Retshjælp (12) 
                GFRequest.HSproduct.HSproductLine.HSobject.risk.retshjaelp = new risk_generic();
                GFRequest.HSproduct.HSproductLine.HSobject.risk.retshjaelp.risk_excess = newSelvrisko;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.retshjaelp.risk_excessSpecified = true;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.retshjaelp.risk_yn = yes_no.Y;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.retshjaelp.risk_ynSpecified = true;

                // Husejeransvar (13)
                GFRequest.HSproduct.HSproductLine.HSobject.risk.husejeransvar = new risk_generic();
                GFRequest.HSproduct.HSproductLine.HSobject.risk.husejeransvar.risk_excess = newSelvrisko;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.husejeransvar.risk_excessSpecified = true;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.husejeransvar.risk_yn = yes_no.Y;
                GFRequest.HSproduct.HSproductLine.HSobject.risk.husejeransvar.risk_ynSpecified = true;
                #endregion mandatory risks

                #region optional risks
                if (FPRequest.insurancerequest.hus.svampinsekt)
                {
                    // Svamp og insekt (3) 
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.svamp_og_insekt = new risk_generic();
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.svamp_og_insekt.risk_excess = newSelvrisko;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.svamp_og_insekt.risk_excessSpecified = true;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.svamp_og_insekt.risk_yn = yes_no.Y;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.svamp_og_insekt.risk_ynSpecified = true;
                }
                if (FPRequest.insurancerequest.hus.raad)
                {
                    // Råd (6)
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.raad = new risk_generic();
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.raad.risk_excess = newSelvrisko;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.raad.risk_excessSpecified = true;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.raad.risk_yn = yes_no.Y;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.raad.risk_ynSpecified = true;
                }
                if (FPRequest.insurancerequest.hus.roerkablerstikledninger)
                {
                    // Rør og Kabel (4) 
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.roer_og_kabel = new risk_generic();
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.roer_og_kabel.risk_excess = newSelvrisko;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.roer_og_kabel.risk_excessSpecified = true;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.roer_og_kabel.risk_yn = yes_no.Y;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.roer_og_kabel.risk_ynSpecified = true;

                    // Stikledning (5)
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.stikledning = new risk_generic();
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.stikledning.risk_excess = newSelvrisko;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.stikledning.risk_excessSpecified = true;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.stikledning.risk_yn = yes_no.Y;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.stikledning.risk_ynSpecified = true;
                }
                if (FPRequest.insurancerequest.hus.udvidetvand)
                {
                    // Udvidet vandskade (15)
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.udvidet_vandskade = new risk_generic();
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.udvidet_vandskade.risk_excess = newSelvrisko;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.udvidet_vandskade.risk_excessSpecified = true;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.udvidet_vandskade.risk_yn = yes_no.Y;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.udvidet_vandskade.risk_ynSpecified = true;
                }
                if (FPRequest.insurancerequest.hus.kosmetiskdaekning)
                {
                    // Kosmetiske forskelle (14)
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.kosmetiske_forskelle = new risk_generic();
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.kosmetiske_forskelle.risk_excess = newSelvrisko;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.kosmetiske_forskelle.risk_excessSpecified = true;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.kosmetiske_forskelle.risk_yn = yes_no.Y;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.kosmetiske_forskelle.risk_ynSpecified = true;
                }
                if (FPRequest.insurancerequest.hus.dyrskader)
                {
                    // Udvidet dækning (16)
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.udvidet_daekning = new risk_generic();
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.udvidet_daekning.risk_excess = newSelvrisko;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.udvidet_daekning.risk_excessSpecified = true;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.udvidet_daekning.risk_yn = yes_no.Y;
                    GFRequest.HSproduct.HSproductLine.HSobject.risk.udvidet_daekning.risk_ynSpecified = true;
                }
                #endregion optional risks
                #endregion risks
            }
            catch (Exception exc)
            {
                ErrorsLogger.Error(new ErrorInfo(exc.ToString()));
                GFRequest = null;
                return false;
            }
            return true;
        }

        public static bool ConvertUlykkeRequest(request FPRequest, out UL_request GFRequest, out string error)
        {
            error = null;

            try
            {
                TIAReferenceServiceClient client = new TIAReferenceServiceClient();

                tariff_codes_request stillingsbet_req = new tariff_codes_request();
                tariff_codes_response stillingsbet_res = new tariff_codes_response();

                stillingsbet_req.product_line_id = "GF_POL";
                stillingsbet_req.table_name = "STILLING";
                stillingsbet_req.code = Convert.ToString(FPRequest.insurancerequest.ulykke.stillingsbetegnelseId);
                stillingsbet_req.indexed = "N";
                if (!client.GetTariffCode(stillingsbet_req, out stillingsbet_res, out error))
                {
                    GFRequest = null;
                    return false;
                }

                if (stillingsbet_res.tariff_code_record == null)
                {
                    GFRequest = null;
                    error = String.Format("Stillings betegnelse {0} eksister ikke hos GF", FPRequest.insurancerequest.ulykke.stillingsbetegnelse);
                    return false;
                }
                if (Convert.ToInt32(stillingsbet_res.tariff_code_record[0].min_val) < 0)
                {
                    GFRequest = null;
                    error = String.Format("Stillings betegnelse {0} eksister ikke hos GF", FPRequest.insurancerequest.ulykke.stillingsbetegnelse);
                    return false;
                }


                GFRequest = new UL_request();
                GFRequest.ULproduct = new ULproduct();
                List<ULproductLine> ulPL = new List<ULproductLine>();

                ulPL.Add(new ULproductLine());
                ulPL[ulPL.Count - 1].ULobject = new ULobject();
                ulPL[ulPL.Count - 1].ULobject.parameters = new parameters_UL();
                ulPL[ulPL.Count - 1].ULobject.risk = new risk_ul();

                ulPL[ulPL.Count - 1].affinity = FPRequest.affinity_no;
                ulPL[ulPL.Count - 1].affinitySpecified = true;

                ulPL[ulPL.Count - 1].ULobject.parameters.person_age = FPRequest.customer.alder;

                if (FPRequest.insurancerequest.ulykke.samlever != null || FPRequest.insurancerequest.ulykke.boern != null)
                {
                    ulPL[ulPL.Count - 1].ULobject.parameters.type_of_accident = insurance_list.Item1;
                }
                else
                {
                    ulPL[ulPL.Count - 1].ULobject.parameters.type_of_accident = insurance_list.Item2;
                }

                if (Convert.ToInt32(stillingsbet_res.tariff_code_record[0].min_val) == 0)
                {
                    ulPL[ulPL.Count - 1].ULobject.parameters.type_of_coverage = coverage_list.Item1;
                }
                else if (FPRequest.insurancerequest.ulykke.heltidsulykke && Convert.ToInt32(stillingsbet_res.tariff_code_record[0].min_val) > 0)
                {
                    ulPL[ulPL.Count - 1].ULobject.parameters.type_of_coverage = coverage_list.Item1;
                }
                else
                {
                    ulPL[ulPL.Count - 1].ULobject.parameters.type_of_coverage = coverage_list.Item2;
                }

                ulPL[ulPL.Count - 1].ULobject.parameters.fareklasse = Convert.ToInt32(stillingsbet_res.tariff_code_record[0].min_val);
                ulPL[ulPL.Count - 1].ULobject.parameters.fareklasseSpecified = true;
                //ulPL[ulPL.Count - 1].ULobject.parameters.occupation = null;

                ulPL[ulPL.Count - 1].ULobject.parameters.addAllRisks = yes_no.N;

                if (FPRequest.insurancerequest.ulykke.forsikringssum_varigt_men > 0)
                {
                    ulPL[ulPL.Count - 1].ULobject.risk.varigt_men = new risk_generic();
                    ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_excess = 0;
                    ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_excessSpecified = true;
                    ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_sum = (int)FPRequest.insurancerequest.ulykke.forsikringssum_varigt_men;
                    ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_sumSpecified = true;
                    ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_yn = yes_no.Y;
                    ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_ynSpecified = true;

                }

                if (FPRequest.insurancerequest.ulykke.forsikringssum_doed > 0)
                {
                    ulPL[ulPL.Count - 1].ULobject.risk.doed = new risk_generic();
                    ulPL[ulPL.Count - 1].ULobject.risk.doed.risk_excess = 0;
                    ulPL[ulPL.Count - 1].ULobject.risk.doed.risk_excessSpecified = true;
                    ulPL[ulPL.Count - 1].ULobject.risk.doed.risk_sum = (int)FPRequest.insurancerequest.ulykke.forsikringssum_doed;
                    ulPL[ulPL.Count - 1].ULobject.risk.doed.risk_sumSpecified = true;
                    ulPL[ulPL.Count - 1].ULobject.risk.doed.risk_yn = yes_no.Y;
                    ulPL[ulPL.Count - 1].ULobject.risk.doed.risk_ynSpecified = true;

                }

                ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning = new risk_generic();
                ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_excess = 0;
                ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_excessSpecified = true;
                ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_sum = 0;
                ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_sumSpecified = true;
                ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_yn = yes_no.Y;
                ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_ynSpecified = true;

                if (FPRequest.insurancerequest.ulykke.farligsport)
                {
                    ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport = new risk_generic();
                    ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_excess = 0;
                    ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_excessSpecified = true;
                    ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_sum = 0;
                    ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_sumSpecified = true;
                    ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_yn = yes_no.Y;
                    ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_ynSpecified = true;
                }

                if (FPRequest.insurancerequest.ulykke.samlever != null)
                {
                    stillingsbet_req.code = Convert.ToString(FPRequest.insurancerequest.ulykke.samlever.stillingsbetegnelseId);
                    stillingsbet_req.indexed = "N";
                    if (!client.GetTariffCode(stillingsbet_req, out stillingsbet_res, out error))
                    {
                        GFRequest = null;
                        return false;
                    }

                    if (stillingsbet_res.tariff_code_record.Length == 0)
                    {
                        GFRequest = null;
                        error = String.Format("Stillings betegnelse {0} eksister ikke hos GF", FPRequest.insurancerequest.ulykke.samlever.stillingsbetegnelse);
                        return false;
                    }
                    if (Convert.ToInt32(stillingsbet_res.tariff_code_record[0].min_val) < 0)
                    {
                        GFRequest = null;
                        error = String.Format("Stillings betegnelse {0} eksister ikke hos GF", FPRequest.insurancerequest.ulykke.samlever.stillingsbetegnelse);
                        return false;
                    }

                    ulPL.Add(new ULproductLine());

                    ulPL[ulPL.Count - 1] = new ULproductLine();
                    ulPL[ulPL.Count - 1].ULobject = new ULobject();
                    ulPL[ulPL.Count - 1].ULobject.parameters = new parameters_UL();
                    ulPL[ulPL.Count - 1].ULobject.risk = new risk_ul();

                    ulPL[ulPL.Count - 1].affinity = FPRequest.affinity_no;
                    ulPL[ulPL.Count - 1].affinitySpecified = true;

                    ulPL[ulPL.Count - 1].ULobject.parameters.person_age = FPRequest.insurancerequest.ulykke.samlever.alder;
                    ulPL[ulPL.Count - 1].ULobject.parameters.type_of_accident = ulPL[0].ULobject.parameters.type_of_accident;

                    if (Convert.ToInt32(stillingsbet_res.tariff_code_record[0].min_val) == 0)
                    {
                        ulPL[ulPL.Count - 1].ULobject.parameters.type_of_coverage = coverage_list.Item1;
                    }
                    else if (FPRequest.insurancerequest.ulykke.samlever.heltidsulykke && Convert.ToInt32(stillingsbet_res.tariff_code_record[0].min_val) > 0)
                    {
                        ulPL[ulPL.Count - 1].ULobject.parameters.type_of_coverage = coverage_list.Item1;
                    }
                    else
                    {
                        ulPL[ulPL.Count - 1].ULobject.parameters.type_of_coverage = coverage_list.Item2;
                    }

                    ulPL[ulPL.Count - 1].ULobject.parameters.fareklasse = Convert.ToInt32(stillingsbet_res.tariff_code_record[0].min_val);
                    ulPL[ulPL.Count - 1].ULobject.parameters.fareklasseSpecified = true;
                    //ulPL[ulPL.Count - 1].ULobject.parameters.occupation = occupation.

                    if (FPRequest.insurancerequest.ulykke.samlever.forsikringssum_varigt_men > 0)
                    {
                        ulPL[ulPL.Count - 1].ULobject.risk.varigt_men = new risk_generic();
                        ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_excess = 0;
                        ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_excessSpecified = true;
                        ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_sum = (int)FPRequest.insurancerequest.ulykke.samlever.forsikringssum_varigt_men;
                        ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_sumSpecified = true;
                        ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_yn = yes_no.Y;
                        ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_ynSpecified = true;
                    }

                    if (FPRequest.insurancerequest.ulykke.samlever.forsikringssum_doed > 0)
                    {
                        ulPL[ulPL.Count - 1].ULobject.risk.doed = new risk_generic();
                        ulPL[ulPL.Count - 1].ULobject.risk.doed.risk_excess = 0;
                        ulPL[ulPL.Count - 1].ULobject.risk.doed.risk_excessSpecified = true;
                        ulPL[ulPL.Count - 1].ULobject.risk.doed.risk_sum = (int)FPRequest.insurancerequest.ulykke.samlever.forsikringssum_doed;
                        ulPL[ulPL.Count - 1].ULobject.risk.doed.risk_sumSpecified = true;
                        ulPL[ulPL.Count - 1].ULobject.risk.doed.risk_yn = yes_no.Y;
                        ulPL[ulPL.Count - 1].ULobject.risk.doed.risk_ynSpecified = true;
                    }

                    ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning = new risk_generic();
                    ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_excess = 0;
                    ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_excessSpecified = true;
                    ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_sum = 0;
                    ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_sumSpecified = true;
                    ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_yn = yes_no.Y;
                    ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_ynSpecified = true;

                    if (FPRequest.insurancerequest.ulykke.samlever.farligsport)
                    {
                        ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport = new risk_generic();
                        ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_excess = 0;
                        ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_excessSpecified = true;
                        ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_sum = 0;
                        ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_sumSpecified = true;
                        ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_yn = yes_no.Y;
                        ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_ynSpecified = true;
                    }
                }


                if (FPRequest.insurancerequest.ulykke.boern != null)
                {
                    for (int i = 0; i < FPRequest.insurancerequest.ulykke.boern.antal_u18; i++)
                    {
                        ulPL.Add(new ULproductLine());

                        ulPL[ulPL.Count - 1] = new ULproductLine();
                        ulPL[ulPL.Count - 1].ULobject = new ULobject();
                        ulPL[ulPL.Count - 1].ULobject.parameters = new parameters_UL();
                        ulPL[ulPL.Count - 1].ULobject.risk = new risk_ul();

                        ulPL[ulPL.Count - 1].affinity = FPRequest.affinity_no;
                        ulPL[ulPL.Count - 1].affinitySpecified = true;

                        if (FPRequest.insurancerequest.ulykke.boern.alder == null)
                        {
                            ulPL[ulPL.Count - 1].ULobject.parameters.person_age = 10; //default value from old algoritme
                        }
                        else
                        {
                            ulPL[ulPL.Count - 1].ULobject.parameters.person_age = FPRequest.insurancerequest.ulykke.boern.alder[i];
                        }

                        ulPL[ulPL.Count - 1].ULobject.parameters.type_of_accident = ulPL[0].ULobject.parameters.type_of_accident;
                        ulPL[ulPL.Count - 1].ULobject.parameters.type_of_coverage = coverage_list.Item1;

                        ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning = new risk_generic();
                        ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_excess = 0;
                        ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_excessSpecified = true;
                        ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_sum = 0;
                        ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_sumSpecified = true;
                        ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_yn = yes_no.Y;
                        ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_ynSpecified = true;


                        if (FPRequest.insurancerequest.ulykke.boern.forsikringssum_varigt_men > 0)
                        {
                            ulPL[ulPL.Count - 1].ULobject.risk.varigt_men = new risk_generic();
                            ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_excess = 0;
                            ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_excessSpecified = true;
                            ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_sum = (int)FPRequest.insurancerequest.ulykke.boern.forsikringssum_varigt_men;
                            ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_sumSpecified = true;
                            ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_yn = yes_no.Y;
                            ulPL[ulPL.Count - 1].ULobject.risk.varigt_men.risk_ynSpecified = true;
                        }

                        if (FPRequest.insurancerequest.ulykke.boern.farligsport)
                        {
                            ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning = new risk_generic();
                            ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_excess = 0;
                            ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_excessSpecified = true;
                            ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_sum = 0;
                            ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_sumSpecified = true;
                            ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_yn = yes_no.Y;
                            ulPL[ulPL.Count - 1].ULobject.risk.udvidet_daekning.risk_ynSpecified = true;

                            ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport = new risk_generic();
                            ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_excess = 0;
                            ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_excessSpecified = true;
                            ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_sum = 0;
                            ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_sumSpecified = true;
                            ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_yn = yes_no.Y;
                            ulPL[ulPL.Count - 1].ULobject.risk.farlig_sport.risk_ynSpecified = true;
                        }
                    }
                }

                GFRequest.ULproduct.ULproductLine = ulPL.ToArray();
            }
            catch (Exception exc)
            {
                ErrorsLogger.Error(new ErrorInfo(exc.ToString()));                
                GFRequest = null;
                return false;
            }
            return true;
        }

        public static bool ConvertSommerhusRequest(request FPRequest, out SO_request GFRequest, out string error)
        {
            int newSelvrisko = 0;
            long newIndbo = 0;

            error = null;

            try
            {
                TIAReferenceServiceClient client = new TIAReferenceServiceClient();

                //tariff_codes_request indbo_sum_req = new tariff_codes_request();
                //tariff_codes_response indbo_sum_res = new tariff_codes_response();

                tariff_codes_request selvrisko_req = new tariff_codes_request();
                tariff_codes_response selvrisko_res = new tariff_codes_response();

                BbrDataResponse bbrResponse;
                KvhxBackendServiceClient kvhxClient = new KvhxBackendServiceClient();

                //indbo_sum_req.product_line_id = "SO";
                //indbo_sum_req.table_name = "PR_SUM";
                //indbo_sum_req.order_by = "MAX_VALUE";
                //indbo_sum_req.indexed = "Y";
                //if (!client.GetTariffCode(indbo_sum_req, out indbo_sum_res, out error))
                //{
                //    GFRequest = null;
                //    return false;
                //}
                //else if (indbo_sum_res.error_record != null)
                //{
                //    error = indbo_sum_res.error_record.error_description;
                //    GFRequest = null;
                //    return false;
                //}

                selvrisko_req.product_line_id = "SO";
                selvrisko_req.table_name = "SELVRISIKO";
                selvrisko_req.order_by = "MAX_VALUE";
                selvrisko_req.indexed = "Y";
                if (!client.GetTariffCode(selvrisko_req, out selvrisko_res, out error))
                {
                    GFRequest = null;
                    return false;
                }
                else if (selvrisko_res.error_record != null)
                {
                    error = selvrisko_res.error_record.error_description;
                    GFRequest = null;
                    return false;
                }

                string newKVHX;
                bbrResponse = GetBBRData(FPRequest.insurancerequest.fritidshus.fritidshus_kvhx, out newKVHX);

                if (bbrResponse.BbrDataEntries.Length == 0)
                {
                    SendMail(FPRequest.insurancerequest.fritidshus.fritidshus_kvhx.ToUpper(), FPRequest.insurancerequest.fritidshus.fritidshus_kvhx.ToUpper());
                    error = "KVHX Index not correct";
                    GFRequest = null;
                    return false;
                }

                if (Int32.Parse(bbrResponse.BbrDataEntries[0].YdervaegsmaterialeKode) == 4 && FPRequest.insurancerequest.fritidshus.svampinsekt)
                {
                    GFRequest = null;
                    error = "";
                    return false;
                }

                GFRequest = new SO_request();
                GFRequest.SOproduct = new SOproduct();
                GFRequest.SOproduct.SOproductLine = new SOproductLine();
                GFRequest.SOproduct.SOproductLine.SOobject = new SOobject();
                GFRequest.SOproduct.SOproductLine.SOobject.parameters = new parameters_so();
                GFRequest.SOproduct.SOproductLine.SOobject.risk = new risk_so();


                GFRequest.SOproduct.SOproductLine.affinity = FPRequest.affinity_no;
                GFRequest.SOproduct.SOproductLine.affinitySpecified = true;

                GFRequest.SOproduct.SOproductLine.SOobject.parameters.basement = yes_no2.N;
                GFRequest.SOproduct.SOproductLine.SOobject.parameters.ejerskifte = yes_no.N;
                GFRequest.SOproduct.SOproductLine.SOobject.parameters.water_stop = yes_no.N;
                GFRequest.SOproduct.SOproductLine.SOobject.parameters.cast_foundation = yes_no2.N;
                GFRequest.SOproduct.SOproductLine.SOobject.parameters.burglar_alarm = 1;


                if (FPRequest.insurancerequest.fritidshus.selvrisiko > Convert.ToUInt16(selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].max_val))
                {
                    //If requested excess is bigger than GF's highest valid excess, set the general_excess to highest excess automatically without the rest of validation.
                    GFRequest.SOproduct.SOproductLine.SOobject.parameters.general_excess = Convert.ToInt32(selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].code);
                    newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].max_val);
                }
                else
                {
                    /*for (int i = selvrisko_res.tariff_code_record.Length; i > 0; i--)
                    {
                        if (FPRequest.insurancerequest.fritidshus.selvrisiko <= Convert.ToUInt16(selvrisko_res.tariff_code_record[i - 1].max_val))
                        {
                            switch (Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].code))
                            {
                                case 6: GFRequest.SOproduct.SOproductLine.SOobject.parameters.general_excess = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].code); newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val); break;
                                case 5: GFRequest.SOproduct.SOproductLine.SOobject.parameters.general_excess = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].code); newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val); break;
                                case 4: GFRequest.SOproduct.SOproductLine.SOobject.parameters.general_excess = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].code); newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val); break;
                                case 3: GFRequest.SOproduct.SOproductLine.SOobject.parameters.general_excess = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].code); newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val); break;
                                case 2: GFRequest.SOproduct.SOproductLine.SOobject.parameters.general_excess = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].code); newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val); break;
                                case 1: GFRequest.SOproduct.SOproductLine.SOobject.parameters.general_excess = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].code); newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val); break;
                                case 0: GFRequest.SOproduct.SOproductLine.SOobject.parameters.general_excess = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].code); newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val); break;
                            }
                        }
                    }*/
                    for (int i = 0; i < selvrisko_res.tariff_code_record.Length; i++)
                    {
                        if (FPRequest.insurancerequest.fritidshus.selvrisiko >= Convert.ToUInt16(selvrisko_res.tariff_code_record[i].max_val) && FPRequest.insurancerequest.fritidshus.selvrisiko <= Convert.ToUInt16(selvrisko_res.tariff_code_record[i + 1].max_val))
                        {
                            if (Math.Abs(FPRequest.insurancerequest.fritidshus.selvrisiko - Convert.ToUInt16(selvrisko_res.tariff_code_record[i].max_val)) < Math.Abs(FPRequest.insurancerequest.fritidshus.selvrisiko - Convert.ToUInt16(selvrisko_res.tariff_code_record[i + 1].max_val)))
                            {
                                newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i].max_val);
                                GFRequest.SOproduct.SOproductLine.SOobject.parameters.general_excess = Convert.ToInt32(selvrisko_res.tariff_code_record[i].code);
                            }
                            else
                            {
                                newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i + 1].max_val);
                                GFRequest.SOproduct.SOproductLine.SOobject.parameters.general_excess = Convert.ToInt32(selvrisko_res.tariff_code_record[i + 1].code);
                            }                            
                            break;
                        }
                    }

                }

                if (FPRequest.insurancerequest.fritidshus.indbosum < Int32.Parse(ConfigurationManager.AppSettings["SOIndboMin"]))
                {
                    newIndbo = Int32.Parse(ConfigurationManager.AppSettings["SOIndboMin"]);
                }
                else if (FPRequest.insurancerequest.fritidshus.indbosum > Int32.Parse(ConfigurationManager.AppSettings["SOIndboMax"]))
                {
                    newIndbo = Int32.Parse(ConfigurationManager.AppSettings["SOIndboMax"]);
                }
                else
                {
                    newIndbo = FPRequest.insurancerequest.fritidshus.indbosum;
                }

                GFRequest.SOproduct.SOproductLine.SOobject.parameters.const_year = FPRequest.insurancerequest.fritidshus.opfoerelsesaar.ToString();
                GFRequest.SOproduct.SOproductLine.SOobject.parameters.ar_living_qrtr = FPRequest.insurancerequest.fritidshus.bebygget_m2;
                GFRequest.SOproduct.SOproductLine.SOobject.parameters.ar_prm_calc = FPRequest.insurancerequest.fritidshus.bebygget_m2;
                GFRequest.SOproduct.SOproductLine.SOobject.parameters.ar_outhouse = FPRequest.insurancerequest.fritidshus.garage_m2;

                switch (FPRequest.insurancerequest.fritidshus.opvarmning)
                {
                    //We are only interesting in items 6 & 9 because those two are not acceptable by GF
                    //case HeatingCodeType.Item1: break;
                    //case HeatingCodeType.Item2: break;
                    //case HeatingCodeType.Item3: break;
                    //case HeatingCodeType.Item4: break;
                    case HeatingCodeType.Item6: GFRequest.SOproduct.SOproductLine.SOobject.parameters.standard_heating = yes_no2.N; GFRequest.SOproduct.SOproductLine.SOobject.parameters.other_heating = heating.Item4; break;
                    //case HeatingCodeType.Item7: break;
                    case HeatingCodeType.Item9: GFRequest.SOproduct.SOproductLine.SOobject.parameters.standard_heating = yes_no2.N; GFRequest.SOproduct.SOproductLine.SOobject.parameters.other_heating = heating.Item4; break;
                    //case HeatingCodeType.Item51: break;
                    //case HeatingCodeType.Item55: break;
                    default: GFRequest.SOproduct.SOproductLine.SOobject.parameters.standard_heating = yes_no2.J; GFRequest.SOproduct.SOproductLine.SOobject.parameters.other_heating = heating.Item1; break;
                }

                switch (FPRequest.insurancerequest.fritidshus.tagbeklaedning)
                {
                    //We are only interesting in item 7 because this one is not acceptable by GF
                    //case RoofingMaterialCodeType.Item1: break;
                    //case RoofingMaterialCodeType.Item2: break;
                    //case RoofingMaterialCodeType.Item3: break;
                    //case RoofingMaterialCodeType.Item4: break;
                    //case RoofingMaterialCodeType.Item5: break;
                    //case RoofingMaterialCodeType.Item6: break;
                    case RoofingMaterialCodeType.Item7: GFRequest.SOproduct.SOproductLine.SOobject.parameters.roof_type = roof_type.Item3; break;
                    //case RoofingMaterialCodeType.Item10: break;
                    //case RoofingMaterialCodeType.Item11: break;
                    //case RoofingMaterialCodeType.Item12: break;
                    //case RoofingMaterialCodeType.Item20: break;
                    //case RoofingMaterialCodeType.Item80: break;
                    //case RoofingMaterialCodeType.Item90: break;
                    default: GFRequest.SOproduct.SOproductLine.SOobject.parameters.roof_type = roof_type.Item1; break;
                }


                GFRequest.SOproduct.SOproductLine.SOobject.parameters.const_year = Convert.ToString(FPRequest.insurancerequest.fritidshus.opfoerelsesaar);

                if (FPRequest.insurancerequest.fritidshus.etager > 1)
                {
                    GFRequest.SOproduct.SOproductLine.SOobject.parameters.no_of_floors = no_of_floors.Item2;
                }
                else
                {
                    GFRequest.SOproduct.SOproductLine.SOobject.parameters.no_of_floors = no_of_floors.Item1;
                }

                switch (FPRequest.insurancerequest.fritidshus.varmeinstallation)
                {
                }

                if (FPRequest.insurancerequest.fritidshus.nedlagt_landbrug)
                {
                    GFRequest.SOproduct.SOproductLine.SOobject.parameters.an_abandoned_agriculture = yes_no.Y;
                }
                else
                {
                    GFRequest.SOproduct.SOproductLine.SOobject.parameters.an_abandoned_agriculture = yes_no.N;
                }
                if (FPRequest.insurancerequest.fritidshus.pool_m2 > 0)
                {
                    GFRequest.SOproduct.SOproductLine.SOobject.parameters.is_indoor_swimming_pool = yes_no2.J;
                }
                else
                {
                    GFRequest.SOproduct.SOproductLine.SOobject.parameters.is_indoor_swimming_pool = yes_no2.N;
                }
                if (FPRequest.insurancerequest.fritidshus.udlejes)
                {
                    GFRequest.SOproduct.SOproductLine.SOobject.parameters.rented_house_indication = yes_no2.J;
                }
                else
                {
                    GFRequest.SOproduct.SOproductLine.SOobject.parameters.rented_house_indication = yes_no2.N;
                }
                GFRequest.SOproduct.SOproductLine.SOobject.parameters.addAllRisks = yes_no.N;

                GFRequest.SOproduct.SOproductLine.SOobject.risk.bygningsbrand = new risk_generic();
                GFRequest.SOproduct.SOproductLine.SOobject.risk.bygningsbrand.risk_excess = newSelvrisko;
                GFRequest.SOproduct.SOproductLine.SOobject.risk.bygningsbrand.risk_excessSpecified = true;
                GFRequest.SOproduct.SOproductLine.SOobject.risk.bygningsbrand.risk_yn = yes_no.Y;
                GFRequest.SOproduct.SOproductLine.SOobject.risk.bygningsbrand.risk_ynSpecified = true;
                GFRequest.SOproduct.SOproductLine.SOobject.risk.bygningsbrand.risk_sum = 0;//(int)newIndbo;
                GFRequest.SOproduct.SOproductLine.SOobject.risk.bygningsbrand.risk_sumSpecified = false;

                GFRequest.SOproduct.SOproductLine.SOobject.risk.bygningskasko = new risk_generic();
                GFRequest.SOproduct.SOproductLine.SOobject.risk.bygningskasko.risk_excess = newSelvrisko;
                GFRequest.SOproduct.SOproductLine.SOobject.risk.bygningskasko.risk_excessSpecified = true;
                GFRequest.SOproduct.SOproductLine.SOobject.risk.bygningskasko.risk_yn = yes_no.Y;
                GFRequest.SOproduct.SOproductLine.SOobject.risk.bygningskasko.risk_ynSpecified = true;
                GFRequest.SOproduct.SOproductLine.SOobject.risk.bygningskasko.risk_sum = 0;
                GFRequest.SOproduct.SOproductLine.SOobject.risk.bygningskasko.risk_sumSpecified = false;

                GFRequest.SOproduct.SOproductLine.SOobject.risk.elskade = new risk_generic();
                GFRequest.SOproduct.SOproductLine.SOobject.risk.elskade.risk_excess = newSelvrisko;
                GFRequest.SOproduct.SOproductLine.SOobject.risk.elskade.risk_excessSpecified = true;
                GFRequest.SOproduct.SOproductLine.SOobject.risk.elskade.risk_yn = yes_no.Y;
                GFRequest.SOproduct.SOproductLine.SOobject.risk.elskade.risk_ynSpecified = true;
                GFRequest.SOproduct.SOproductLine.SOobject.risk.elskade.risk_sum = 0;
                GFRequest.SOproduct.SOproductLine.SOobject.risk.elskade.risk_sumSpecified = false;      

                if (FPRequest.insurancerequest.fritidshus.indbosum > 0)
                {
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.indbo = new risk_generic();
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.indbo.risk_excess = newSelvrisko;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.indbo.risk_excessSpecified = true;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.indbo.risk_yn = yes_no.Y;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.indbo.risk_ynSpecified = true;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.indbo.risk_sum = (int)newIndbo;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.indbo.risk_sumSpecified = true;
                }

                if (FPRequest.insurancerequest.fritidshus.raad)
                {
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.raad = new risk_generic();
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.raad.risk_excess = newSelvrisko;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.raad.risk_excessSpecified = true;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.raad.risk_yn = yes_no.Y;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.raad.risk_ynSpecified = true;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.raad.risk_sum = 0;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.raad.risk_sumSpecified = false;
                }

                if (FPRequest.insurancerequest.fritidshus.svampinsekt)
                {
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.svamp_og_insekt = new risk_generic();
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.svamp_og_insekt.risk_excess = newSelvrisko;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.svamp_og_insekt.risk_excessSpecified = true;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.svamp_og_insekt.risk_yn = yes_no.Y;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.svamp_og_insekt.risk_ynSpecified = true;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.svamp_og_insekt.risk_sum = 0;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.svamp_og_insekt.risk_sumSpecified = false;
                }

                if (FPRequest.insurancerequest.fritidshus.roerstikledninger)
                {
                    //GFRequest.SOproduct.SOproductLine.SOobject.risk.stikledning = new risk_generic();
                    //GFRequest.SOproduct.SOproductLine.SOobject.risk.stikledning.risk_excess = newSelvrisko;
                    //GFRequest.SOproduct.SOproductLine.SOobject.risk.stikledning.risk_excessSpecified = true;
                    //GFRequest.SOproduct.SOproductLine.SOobject.risk.stikledning.risk_yn = yes_no.Y;
                    //GFRequest.SOproduct.SOproductLine.SOobject.risk.stikledning.risk_ynSpecified = true;
                    //GFRequest.SOproduct.SOproductLine.SOobject.risk.stikledning.risk_sum = 0;
                    //GFRequest.SOproduct.SOproductLine.SOobject.risk.stikledning.risk_sumSpecified = false;

                    GFRequest.SOproduct.SOproductLine.SOobject.risk.udvidet_roerskade = new risk_generic();
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.udvidet_roerskade.risk_excess = newSelvrisko;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.udvidet_roerskade.risk_excessSpecified = true;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.udvidet_roerskade.risk_yn = yes_no.Y;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.udvidet_roerskade.risk_ynSpecified = true;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.udvidet_roerskade.risk_sum = 0;
                    GFRequest.SOproduct.SOproductLine.SOobject.risk.udvidet_roerskade.risk_sumSpecified = false;
                }
            }
            catch (Exception exc)
            {
                ErrorsLogger.Error(new ErrorInfo(exc.ToString()));                
                GFRequest = null;
                return false;
            }
            return true;
        }

        public static bool ConvertGFHelpRequest(request FPRequest, out GF_request GFRequest, out string error)
        {
            error = null;
            try
            {
                GFRequest = new GF_request();
                GFRequest.GFproduct = new GFproduct();
                GFRequest.GFproduct.GFproductLine = new GFproductLine();
                GFRequest.GFproduct.GFproductLine.GFobject = new GFobject();
                GFRequest.GFproduct.GFproductLine.GFobject.parameters = new parameters_GF();

                GFRequest.GFproduct.GFproductLine.affinity = FPRequest.affinity_no;
                GFRequest.GFproduct.GFproductLine.affinitySpecified = true;

                GFRequest.GFproduct.GFproductLine.GFobject.parameters.car_manufacturer = FPRequest.insurancerequest.bil.fabrikat;
                GFRequest.GFproduct.GFproductLine.GFobject.parameters.car_model_type = FPRequest.insurancerequest.bil.model;
                GFRequest.GFproduct.GFproductLine.GFobject.parameters.car_registration_number = FPRequest.insurancerequest.bil.reg_nr;
                GFRequest.GFproduct.GFproductLine.GFobject.parameters.car_registration_year = FPRequest.insurancerequest.bil.foerste_reg.Year;

                GFRequest.GFproduct.GFproductLine.GFobject.risk = new risk_GF();
                GFRequest.GFproduct.GFproductLine.GFobject.risk.gf_hjaelp = new risk_generic();
                GFRequest.GFproduct.GFproductLine.GFobject.risk.gf_hjaelp.risk_excess = 0;
                GFRequest.GFproduct.GFproductLine.GFobject.risk.gf_hjaelp.risk_excessSpecified = true;
                GFRequest.GFproduct.GFproductLine.GFobject.risk.gf_hjaelp.risk_sum = 0;
                GFRequest.GFproduct.GFproductLine.GFobject.risk.gf_hjaelp.risk_sumSpecified = true;
                GFRequest.GFproduct.GFproductLine.GFobject.risk.gf_hjaelp.risk_yn = yes_no.Y;
                GFRequest.GFproduct.GFproductLine.GFobject.risk.gf_hjaelp.risk_ynSpecified = true;
            }
            catch (Exception exc)
            {
                ErrorsLogger.Error(new ErrorInfo(exc.ToString()));                
                GFRequest = null;
                return false;
            }
            return true;
        }

        public static bool ConvertIndboRequest(request FPRequest, out IN_request GFRequest, out string error)
        {
            int newSelvrisko = 0;
            long newIndbo = 0;
            error = null;
            BbrDataResponse bbrResponse;

            try
            {
                #region Preconditions check
                //artf271144 Start
                if (FPRequest.insurancerequest.indbo.tagbeklaedning == RoofingMaterialCodeType.Item7)
                {
                    error = "GF tegner ikke forsikring for denne type tagbeklædning.";
                    GFRequest = null;
                    return false;
                }
                //artf271144 End
                #endregion

                #region Call to extern WS
                TIAReferenceServiceClient client = new TIAReferenceServiceClient();
                KvhxBackendServiceClient kvhxClient = new KvhxBackendServiceClient();
                
                
                tariff_codes_request indbo_sum_req = new tariff_codes_request();
                tariff_codes_response indbo_sum_res = new tariff_codes_response();

                tariff_codes_request selvrisko_req = new tariff_codes_request();
                tariff_codes_response selvrisko_res = new tariff_codes_response();

                indbo_sum_req.product_line_id = "IN";
                indbo_sum_req.table_name = "SUM_POL";
                indbo_sum_req.order_by = "MAX_VALUE";
                indbo_sum_req.indexed = "Y";
                indbo_sum_req.round = 3;
                indbo_sum_req.roundSpecified = true;
                if (!client.GetTariffCode(indbo_sum_req, out indbo_sum_res, out error))
                {
                    GFRequest = null;
                    return false;
                }
                else if (indbo_sum_res.error_record != null)
                {
                    error = indbo_sum_res.error_record.error_description;
                    GFRequest = null;
                    return false;
                }

                selvrisko_req.product_line_id = "IN";
                selvrisko_req.table_name = "SELV_GENERE";
                selvrisko_req.order_by = "MAX_VALUE";
                selvrisko_req.indexed = "Y";
                if (!client.GetTariffCode(selvrisko_req, out selvrisko_res, out error))
                {
                    GFRequest = null;
                    return false;
                }
                else if (selvrisko_res.error_record != null)
                {
                    error = selvrisko_res.error_record.error_description;
                    GFRequest = null;
                    return false;
                }
                // KKK VVVV HHHH EE SSSS
                // 461 7258 073C st 00th
                // 540 1025 0008 02 00mf
                // 461 7258 073C ST TH
                // 461 3502 019

                string newKVHX;
                bbrResponse = GetBBRData(FPRequest.customer.kvhx, out newKVHX);

                if (bbrResponse.BbrDataEntries.Length == 0)
                {
                    error = "KVHX Index not correct";
                    GFRequest = null;
                    return false;
                }

                #endregion Call to extern WS

                #region Init object
                GFRequest = new IN_request();
                GFRequest.INproduct = new INproduct();
                GFRequest.INproduct.INproductLine = new INproductLine();
                GFRequest.INproduct.INproductLine.INobject = new INobject();
                GFRequest.INproduct.INproductLine.INobject.parameters = new parameters_IN();
                GFRequest.INproduct.INproductLine.INobject.risk = new risk_IN();
                #endregion Init object

                GFRequest.INproduct.INproductLine.affinity = FPRequest.affinity_no;
                GFRequest.INproduct.INproductLine.affinitySpecified = true;

                #region Defaulted parameters
                GFRequest.INproduct.INproductLine.INobject.parameters.addAllRisks = yes_no.N;
                GFRequest.INproduct.INproductLine.INobject.parameters.addAllRisksSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.parameters.rki_yn = "N";
                GFRequest.INproduct.INproductLine.INobject.parameters.locations_share_of_sum = 100;
                GFRequest.INproduct.INproductLine.INobject.parameters.locations_share_of_sumSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.parameters.gf_owner_or_renter = "EJER";
                #endregion Defaulted parameters

                #region Mapning of parameters
                GFRequest.INproduct.INproductLine.INobject.parameters.tariff_user_age = FPRequest.customer.alder;
                GFRequest.INproduct.INproductLine.INobject.parameters.tariff_user_ageSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.parameters.tariff_user_age_at_first_start = FPRequest.customer.alder;
                GFRequest.INproduct.INproductLine.INobject.parameters.tariff_user_age_at_first_startSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.parameters.postal_code = FPRequest.customer.postnr.ToString();
                GFRequest.INproduct.INproductLine.INobject.parameters.damages_last_years = FPRequest.insurancerequest.indbo.antal_skader_treaar;
                GFRequest.INproduct.INproductLine.INobject.parameters.damages_last_yearsSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.parameters.no_of_adults = FPRequest.insurancerequest.indbo.antal_voksne;
                GFRequest.INproduct.INproductLine.INobject.parameters.no_of_adultsSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.parameters.no_of_children = FPRequest.insurancerequest.indbo.antal_boern;
                GFRequest.INproduct.INproductLine.INobject.parameters.no_of_childrenSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.parameters.bbr_kvhx = newKVHX;
                GFRequest.INproduct.INproductLine.INobject.parameters.bbr_zone = bbrResponse.BbrDataEntries[0].ZonekodeKode;

                if (GFRequest.INproduct.INproductLine.INobject.parameters.bbr_zone == "1")
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.gf_zone = "BY";
                }
                else if (GFRequest.INproduct.INproductLine.INobject.parameters.bbr_zone == "2")
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.gf_zone = "LAND";
                }
                else
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.gf_zone = "OVRIGE";
                }

                if (!String.IsNullOrEmpty(bbrResponse.BbrDataEntries[0].Etage) && !String.IsNullOrWhiteSpace(bbrResponse.BbrDataEntries[0].Etage))
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.bbr_floor_no = bbrResponse.BbrDataEntries[0].Etage;
                    //GFRequest.INproduct.INproductLine.INobject.parameters.gf_floor_no = bbrResponse.BbrDataEntries[0].Etage;                    
                }                
                GFRequest.INproduct.INproductLine.INobject.parameters.bbr_basement_areasize = bbrResponse.BbrDataEntries[0].Kaelderareal.Value;
                GFRequest.INproduct.INproductLine.INobject.parameters.bbr_basement_areasizeSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.parameters.bbr_housing_usage = bbrResponse.BbrDataEntries[0].BygningensAnvendelseKode;
                GFRequest.INproduct.INproductLine.INobject.parameters.bbr_no_of_floors = bbrResponse.BbrDataEntries[0].AntalEtagerIBygning.Value;
                GFRequest.INproduct.INproductLine.INobject.parameters.bbr_no_of_floorsSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.parameters.gf_no_of_floors = bbrResponse.BbrDataEntries[0].AntalEtagerIBygning.Value;
                GFRequest.INproduct.INproductLine.INobject.parameters.gf_no_of_floorsSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.parameters.no_of_floors = bbrResponse.BbrDataEntries[0].AntalEtagerIBygning.Value;
                GFRequest.INproduct.INproductLine.INobject.parameters.no_of_floorsSpecified = true;


                /*if (bbrResponse.BbrDataEntries[0].AntalEtagerIBygning.Value.ToString() == bbrResponse.BbrDataEntries[0].Etage)                    
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.gf_floor_grp = "OVERST";
                }
                else if (bbrResponse.BbrDataEntries[0].Etage == "ST" || bbrResponse.BbrDataEntries[0].Etage == "KLD")
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.gf_floor_grp = "NEDERST";
                }
                if (bbrResponse.BbrDataEntries[0].AntalEtagerIBygning.Value.ToString() == bbrResponse.BbrDataEntries[0].Etage)
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.gf_floor_grp = "ANDET";
                }
                else
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.gf_floor_grp = "INGEN";
                }*/

                if (FPRequest.insurancerequest.indbo.kaelder_m2 > 0 && FPRequest.insurancerequest.indbo.kaelder_m2 <= 25)
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.gf_basement = "JA_LILLE";
                }
                else if (FPRequest.insurancerequest.indbo.kaelder_m2 > 25)
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.gf_basement = "JA_STOR";
                }
                else
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.gf_basement = "NEJ";
                }

                switch (FPRequest.insurancerequest.indbo.bo_type)
                {
                    case UseCodeType.Item120: GFRequest.INproduct.INproductLine.INobject.parameters.gf_accomodation_type = "HUS_EJ"; break;
                    case UseCodeType.Item130: GFRequest.INproduct.INproductLine.INobject.parameters.gf_accomodation_type = "RAEKKE"; break;
                    case UseCodeType.Item140: GFRequest.INproduct.INproductLine.INobject.parameters.gf_accomodation_type = "LEJL"; break;
                    default: GFRequest.INproduct.INproductLine.INobject.parameters.gf_accomodation_type = "OVRIGE"; break;
                }

                if (FPRequest.insurancerequest.indbo.tagbeklaedning == RoofingMaterialCodeType.Item7)
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.gf_rooftype = "STRA";
                }
                else
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.gf_rooftype = "HARD";
                }



                if (FPRequest.insurancerequest.indbo.alarm && FPRequest.insurancerequest.indbo.tilknyttet_alarmcentral)
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.burglar_alarm = "MED";
                }
                else if (FPRequest.insurancerequest.indbo.alarm && !FPRequest.insurancerequest.indbo.tilknyttet_alarmcentral)
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.burglar_alarm = "UDEN";
                }
                else
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.burglar_alarm = "INGEN";
                }                


                if (FPRequest.insurancerequest.indbo.selvrisiko > Convert.ToUInt16(selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].max_val))
                {
                    //If requested excess is bigger than GF's highest valid excess, set the general_excess to highest excess automatically without the rest of validation.
                    GFRequest.INproduct.INproductLine.INobject.parameters.general_excess = Convert.ToUInt16(selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].max_val);
                    GFRequest.INproduct.INproductLine.INobject.parameters.general_excessSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.parameters.general_excess_code = selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].code;
                    newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[selvrisko_res.tariff_code_record.Length - 1].max_val);
                }
                else
                {
                    /*for (int i = selvrisko_res.tariff_code_record.Length; i > 0; i--)
                    {
                        if (FPRequest.insurancerequest.indbo.selvrisiko <= Convert.ToUInt16(selvrisko_res.tariff_code_record[i - 1].max_val))
                        {
                            GFRequest.INproduct.INproductLine.INobject.parameters.general_excess = Convert.ToUInt16(selvrisko_res.tariff_code_record[i - 1].max_val);
                            GFRequest.INproduct.INproductLine.INobject.parameters.general_excessSpecified = true;
                            GFRequest.INproduct.INproductLine.INobject.parameters.general_excess_code = selvrisko_res.tariff_code_record[i - 1].code;
                            newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i - 1].max_val);
                        }
                    }*/

                    for (int i = 0; i < selvrisko_res.tariff_code_record.Length; i++)
                    {
                        if (FPRequest.insurancerequest.indbo.selvrisiko >= Convert.ToUInt16(selvrisko_res.tariff_code_record[i].max_val) && FPRequest.insurancerequest.indbo.selvrisiko <= Convert.ToUInt16(selvrisko_res.tariff_code_record[i + 1].max_val))
                        {
                            if (Math.Abs(FPRequest.insurancerequest.indbo.selvrisiko - Convert.ToUInt16(selvrisko_res.tariff_code_record[i].max_val)) < Math.Abs(FPRequest.insurancerequest.indbo.selvrisiko - Convert.ToUInt16(selvrisko_res.tariff_code_record[i + 1].max_val)))
                            {
                                newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i].max_val);
                                GFRequest.INproduct.INproductLine.INobject.parameters.general_excess = newSelvrisko;
                                GFRequest.INproduct.INproductLine.INobject.parameters.general_excessSpecified = true;
                                GFRequest.INproduct.INproductLine.INobject.parameters.general_excess_code = selvrisko_res.tariff_code_record[i].code;
                            }
                            else
                            {
                                newSelvrisko = Convert.ToInt32(selvrisko_res.tariff_code_record[i + 1].max_val);
                                GFRequest.INproduct.INproductLine.INobject.parameters.general_excess = newSelvrisko;
                                GFRequest.INproduct.INproductLine.INobject.parameters.general_excessSpecified = true;
                                GFRequest.INproduct.INproductLine.INobject.parameters.general_excess_code = selvrisko_res.tariff_code_record[i + 1].code;
                            }                            
                            break;
                        }
                    }

                }

                if (FPRequest.insurancerequest.indbo.indbosum > Convert.ToUInt32(indbo_sum_res.tariff_code_record[indbo_sum_res.tariff_code_record.Length - 1].max_val))
                {
                    GFRequest.INproduct.INproductLine.INobject.parameters.policy_sum = Convert.ToInt32(indbo_sum_res.tariff_code_record[indbo_sum_res.tariff_code_record.Length - 1].max_val);
                    GFRequest.INproduct.INproductLine.INobject.parameters.policy_sumSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.parameters.policy_sum_code = indbo_sum_res.tariff_code_record[indbo_sum_res.tariff_code_record.Length - 1].code;
                    newIndbo = Convert.ToInt32(indbo_sum_res.tariff_code_record[indbo_sum_res.tariff_code_record.Length - 1].max_val);                    
                }
                else
                {
                    for (int i = indbo_sum_res.tariff_code_record.Length; i > 0; i--)
                    {
                        if (FPRequest.insurancerequest.indbo.indbosum <= Convert.ToUInt32(indbo_sum_res.tariff_code_record[i - 1].max_val))
                        {
                            GFRequest.INproduct.INproductLine.INobject.parameters.policy_sum = Convert.ToInt32(indbo_sum_res.tariff_code_record[i - 1].max_val);
                            GFRequest.INproduct.INproductLine.INobject.parameters.policy_sumSpecified = true;
                            GFRequest.INproduct.INproductLine.INobject.parameters.policy_sum_code = indbo_sum_res.tariff_code_record[i - 1].code;
                            newIndbo = Convert.ToInt32(indbo_sum_res.tariff_code_record[i - 1].max_val);
                        }

                    }
                }
                #endregion Mapning of parameters


                #region Mandatory Risks
                GFRequest.INproduct.INproductLine.INobject.risk.ansvar = new risk_generic();
                GFRequest.INproduct.INproductLine.INobject.risk.ansvar.risk_excess = newSelvrisko;
                GFRequest.INproduct.INproductLine.INobject.risk.ansvar.risk_excessSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.ansvar.risk_sum = (int)newIndbo;
                GFRequest.INproduct.INproductLine.INobject.risk.ansvar.risk_sumSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.ansvar.risk_yn = yes_no.Y;
                GFRequest.INproduct.INproductLine.INobject.risk.ansvar.risk_ynSpecified = true;

                GFRequest.INproduct.INproductLine.INobject.risk.indbo = new risk_generic();
                GFRequest.INproduct.INproductLine.INobject.risk.indbo.risk_excess = newSelvrisko;
                GFRequest.INproduct.INproductLine.INobject.risk.indbo.risk_excessSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.indbo.risk_sum = (int)newIndbo;
                GFRequest.INproduct.INproductLine.INobject.risk.indbo.risk_sumSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.indbo.risk_yn = yes_no.Y;
                GFRequest.INproduct.INproductLine.INobject.risk.indbo.risk_ynSpecified = true;

                GFRequest.INproduct.INproductLine.INobject.risk.retshjaelp = new risk_generic();
                GFRequest.INproduct.INproductLine.INobject.risk.retshjaelp.risk_excess = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.retshjaelp.risk_excessSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.retshjaelp.risk_sum = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.retshjaelp.risk_sumSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.retshjaelp.risk_yn = yes_no.Y;
                GFRequest.INproduct.INproductLine.INobject.risk.retshjaelp.risk_ynSpecified = true;

                GFRequest.INproduct.INproductLine.INobject.risk.idsikring = new risk_generic();
                GFRequest.INproduct.INproductLine.INobject.risk.idsikring.risk_excess = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.idsikring.risk_excessSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.idsikring.risk_sum = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.idsikring.risk_sumSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.idsikring.risk_yn = yes_no.Y;
                GFRequest.INproduct.INproductLine.INobject.risk.idsikring.risk_ynSpecified = true;

                GFRequest.INproduct.INproductLine.INobject.risk.elskade = new risk_generic();
                GFRequest.INproduct.INproductLine.INobject.risk.elskade.risk_excess = newSelvrisko;
                GFRequest.INproduct.INproductLine.INobject.risk.elskade.risk_excessSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.elskade.risk_sum = (int)newIndbo;
                GFRequest.INproduct.INproductLine.INobject.risk.elskade.risk_sumSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.elskade.risk_yn = yes_no.Y;
                GFRequest.INproduct.INproductLine.INobject.risk.elskade.risk_ynSpecified = true;

                GFRequest.INproduct.INproductLine.INobject.risk.cykelforsikring = new risk_generic();
                GFRequest.INproduct.INproductLine.INobject.risk.cykelforsikring.risk_excess = newSelvrisko;
                GFRequest.INproduct.INproductLine.INobject.risk.cykelforsikring.risk_excessSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.cykelforsikring.risk_sum = (int)newIndbo;
                GFRequest.INproduct.INproductLine.INobject.risk.cykelforsikring.risk_sumSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.cykelforsikring.risk_yn = yes_no.Y;
                GFRequest.INproduct.INproductLine.INobject.risk.cykelforsikring.risk_ynSpecified = true;

                #endregion Mandatory Risks

                #region Optional Risk
                if (FPRequest.insurancerequest.indbo.elektronik)
                {
                    GFRequest.INproduct.INproductLine.INobject.risk.elektronikforsikring = new risk_generic();
                    GFRequest.INproduct.INproductLine.INobject.risk.elektronikforsikring.risk_excess = 0;
                    GFRequest.INproduct.INproductLine.INobject.risk.elektronikforsikring.risk_excessSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.risk.elektronikforsikring.risk_sum = Convert.ToInt32(selvrisko_res.tariff_code_record[1].max_val);
                    GFRequest.INproduct.INproductLine.INobject.risk.elektronikforsikring.risk_sumSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.risk.elektronikforsikring.risk_yn = yes_no.Y;
                    GFRequest.INproduct.INproductLine.INobject.risk.elektronikforsikring.risk_ynSpecified = true;
                }

                if (FPRequest.insurancerequest.indbo.glaskumme)
                {
                    GFRequest.INproduct.INproductLine.INobject.risk.glas_og_sanitet = new risk_generic();
                    GFRequest.INproduct.INproductLine.INobject.risk.glas_og_sanitet.risk_excess = newSelvrisko;
                    GFRequest.INproduct.INproductLine.INobject.risk.glas_og_sanitet.risk_excessSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.risk.glas_og_sanitet.risk_sum = 0;
                    GFRequest.INproduct.INproductLine.INobject.risk.glas_og_sanitet.risk_sumSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.risk.glas_og_sanitet.risk_yn = yes_no.Y;
                    GFRequest.INproduct.INproductLine.INobject.risk.glas_og_sanitet.risk_ynSpecified = true;
                }

                if (FPRequest.insurancerequest.indbo.rejseeuropa)
                {
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__europa_inkl_tyrkiet = new risk_generic();
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__europa_inkl_tyrkiet.risk_excess = 0;
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__europa_inkl_tyrkiet.risk_excessSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__europa_inkl_tyrkiet.risk_sum = 0;
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__europa_inkl_tyrkiet.risk_sumSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__europa_inkl_tyrkiet.risk_yn = yes_no.Y;
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__europa_inkl_tyrkiet.risk_ynSpecified = true;
                }

                if (FPRequest.insurancerequest.indbo.rejseverden)
                {
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__verden = new risk_generic();
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__verden.risk_excess = 0;
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__verden.risk_excessSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__verden.risk_sum = 0;
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__verden.risk_sumSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__verden.risk_yn = yes_no.Y;
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__verden.risk_ynSpecified = true;
                }

                if (FPRequest.insurancerequest.indbo.afbestillingsforsikring)
                {
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__afbestilling = new risk_generic();
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__afbestilling.risk_excess = 0;
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__afbestilling.risk_excessSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__afbestilling.risk_sum = 0;
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__afbestilling.risk_sumSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__afbestilling.risk_yn = yes_no.Y;
                    GFRequest.INproduct.INproductLine.INobject.risk.rejse__afbestilling.risk_ynSpecified = true;
                }

                if (FPRequest.insurancerequest.indbo.pludseligskade)
                {
                    GFRequest.INproduct.INproductLine.INobject.risk.pludselig_skade = new risk_generic();
                    GFRequest.INproduct.INproductLine.INobject.risk.pludselig_skade.risk_excess = 0;
                    GFRequest.INproduct.INproductLine.INobject.risk.pludselig_skade.risk_excessSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.risk.pludselig_skade.risk_sum = Convert.ToInt32(selvrisko_res.tariff_code_record[1].max_val);
                    GFRequest.INproduct.INproductLine.INobject.risk.pludselig_skade.risk_sumSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.risk.pludselig_skade.risk_yn = yes_no.Y;
                    GFRequest.INproduct.INproductLine.INobject.risk.pludselig_skade.risk_ynSpecified = true;

                    GFRequest.INproduct.INproductLine.INobject.risk.briller_og_hoereapparater = new risk_generic();
                    GFRequest.INproduct.INproductLine.INobject.risk.briller_og_hoereapparater.risk_excess = 0;
                    GFRequest.INproduct.INproductLine.INobject.risk.briller_og_hoereapparater.risk_excessSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.risk.briller_og_hoereapparater.risk_sum = Convert.ToInt32(selvrisko_res.tariff_code_record[1].max_val);
                    GFRequest.INproduct.INproductLine.INobject.risk.briller_og_hoereapparater.risk_sumSpecified = true;
                    GFRequest.INproduct.INproductLine.INobject.risk.briller_og_hoereapparater.risk_yn = yes_no.Y;
                    GFRequest.INproduct.INproductLine.INobject.risk.briller_og_hoereapparater.risk_ynSpecified = true;
                }
                #endregion Optional Risk

                #region Not Allowed Risk
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_cykel = new risk_generic();
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_cykel.risk_excess = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_cykel.risk_excessSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_cykel.risk_sum = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_cykel.risk_sumSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_cykel.risk_yn = yes_no.N;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_cykel.risk_ynSpecified = true;

                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_guld_soelv_mm = new risk_generic();
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_guld_soelv_mm.risk_excess = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_guld_soelv_mm.risk_excessSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_guld_soelv_mm.risk_sum = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_guld_soelv_mm.risk_sumSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_guld_soelv_mm.risk_yn = yes_no.N;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_guld_soelv_mm.risk_ynSpecified = true;

                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_indbo_i_udhuse = new risk_generic();
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_indbo_i_udhuse.risk_excess = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_indbo_i_udhuse.risk_excessSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_indbo_i_udhuse.risk_sum = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_indbo_i_udhuse.risk_sumSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_indbo_i_udhuse.risk_yn = yes_no.N;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_indbo_i_udhuse.risk_ynSpecified = true;

                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_vandskade = new risk_generic();
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_vandskade.risk_excess = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_vandskade.risk_excessSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_vandskade.risk_sum = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_vandskade.risk_sumSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_vandskade.risk_yn = yes_no.N;
                GFRequest.INproduct.INproductLine.INobject.risk.udvidet_vandskade.risk_ynSpecified = true;

                GFRequest.INproduct.INproductLine.INobject.risk.udeboende_barn_under_26_aar = new risk_generic();
                GFRequest.INproduct.INproductLine.INobject.risk.udeboende_barn_under_26_aar.risk_excess = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.udeboende_barn_under_26_aar.risk_excessSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.udeboende_barn_under_26_aar.risk_sum = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.udeboende_barn_under_26_aar.risk_sumSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.udeboende_barn_under_26_aar.risk_yn = yes_no.N;
                GFRequest.INproduct.INproductLine.INobject.risk.udeboende_barn_under_26_aar.risk_ynSpecified = true;

                GFRequest.INproduct.INproductLine.INobject.risk.rejse__ski_og_sport = new risk_generic();
                GFRequest.INproduct.INproductLine.INobject.risk.rejse__ski_og_sport.risk_excess = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.rejse__ski_og_sport.risk_excessSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.rejse__ski_og_sport.risk_sum = 0;
                GFRequest.INproduct.INproductLine.INobject.risk.rejse__ski_og_sport.risk_sumSpecified = true;
                GFRequest.INproduct.INproductLine.INobject.risk.rejse__ski_og_sport.risk_yn = yes_no.N;
                GFRequest.INproduct.INproductLine.INobject.risk.rejse__ski_og_sport.risk_ynSpecified = true;
                #endregion Not Allowed Risk
            }
            catch (Exception exc)
            {
                InfosLogger.Info(String.Format("End {0}", DateTime.Now.ToString()), "string");
                ErrorsLogger.Error(new ErrorInfo(exc.ToString()));
                GFRequest = null;
                return false;
            }
            return true;
        }

        private static void SendMail(String KVHX, String Address)
        {
            string Error;

            SendEmailServiceClient EmailCient = new SendEmailServiceClient();
            if (!EmailCient.SendEmail("forsikringsguide@gfforsikring.dk", ConfigurationManager.AppSettings["DispatcherMail"], "KVHX Fejl", String.Format("KVHX ({0}) for adressen {1} fejler.", KVHX, Address), false, out Error))
            {
                ErrorsLogger.Error(new ErrorInfo("Fejl ved afsendelse af mail"));
            }            
        }
        #endregion Public Methods

        #region Private Methods

        private static BbrDataResponse GetBBRData(string kvhx, out string newKVHX)
        {
            KvhxBackendServiceClient kvhxClient = new KvhxBackendServiceClient();

            int houseNumber;
            newKVHX = kvhx.ToUpper().Substring(0, 3); //KKK
            newKVHX += kvhx.ToUpper().Substring(3, 4); //VVVV
            if (int.TryParse(kvhx.ToUpper().Substring(7, 4), out houseNumber))
            {
                newKVHX += kvhx.ToUpper().Substring(7, 4).TrimStart('0').PadLeft(3, '0'); //HHHHH
            }
            else
            {
                newKVHX += kvhx.ToUpper().Substring(7, 4).TrimStart('0').PadLeft(4, '0'); //HHHHH
            }
            if (kvhx.ToUpper().Substring(11, 6).Trim('0').Length > 0)
            {
                newKVHX += kvhx.ToUpper().Substring(11, 2); //EE
                newKVHX += kvhx.ToUpper().Substring(13, 4).TrimStart('0'); //SSSS
                newKVHX = newKVHX.TrimEnd('0');
            }
            InfosLogger.Info(String.Format("Start KVHX {0} Timestamp {1}", newKVHX, DateTime.Now.ToString()), "string");
            return kvhxClient.GetBbrDataFromKvhx(newKVHX);
        }
        #endregion

    }
}