﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PriceCalculatorServiceClient.IPriceCalcService")]
    public interface IPriceCalcService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPriceCalcService/CalculatePrice", ReplyAction="http://tempuri.org/IPriceCalcService/CalculatePriceResponse")]
        UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.CalculatePriceResponse CalculatePrice(UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.CalculatePriceRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPriceCalcService/CalculatePrice", ReplyAction="http://tempuri.org/IPriceCalcService/CalculatePriceResponse")]
        System.Threading.Tasks.Task<UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.CalculatePriceResponse> CalculatePriceAsync(UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.CalculatePriceRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CalculatePrice", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class CalculatePriceRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string xmlRequest;
        
        public CalculatePriceRequest() {
        }
        
        public CalculatePriceRequest(string xmlRequest) {
            this.xmlRequest = xmlRequest;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CalculatePriceResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class CalculatePriceResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public bool CalculatePriceResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string xmlResponse;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public string error;
        
        public CalculatePriceResponse() {
        }
        
        public CalculatePriceResponse(bool CalculatePriceResult, string xmlResponse, string error) {
            this.CalculatePriceResult = CalculatePriceResult;
            this.xmlResponse = xmlResponse;
            this.error = error;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPriceCalcServiceChannel : UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.IPriceCalcService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PriceCalcServiceClient : System.ServiceModel.ClientBase<UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.IPriceCalcService>, UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.IPriceCalcService {
        
        public PriceCalcServiceClient() {
        }
        
        public PriceCalcServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PriceCalcServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PriceCalcServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PriceCalcServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.CalculatePriceResponse UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.IPriceCalcService.CalculatePrice(UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.CalculatePriceRequest request) {
            return base.Channel.CalculatePrice(request);
        }
        
        public bool CalculatePrice(string xmlRequest, out string xmlResponse, out string error) {
            UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.CalculatePriceRequest inValue = new UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.CalculatePriceRequest();
            inValue.xmlRequest = xmlRequest;
            UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.CalculatePriceResponse retVal = ((UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.IPriceCalcService)(this)).CalculatePrice(inValue);
            xmlResponse = retVal.xmlResponse;
            error = retVal.error;
            return retVal.CalculatePriceResult;
        }
        
        public System.Threading.Tasks.Task<UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.CalculatePriceResponse> CalculatePriceAsync(UnitTest.FandP.PriceCalculator.PriceCalculatorServiceClient.CalculatePriceRequest request) {
            return base.Channel.CalculatePriceAsync(request);
        }
    }
}
