svcutil.exe /t:metadata \\ocn-rd-swcetest\ami\main\bin\Itron.Ami.Facade.WebServices.Internal.dll /et:Itron.Ami.Facade.WebServices.Internal.IServiceBase
svcutil.exe /noconfig /namespace:*,Itron.Ami.Facade.WebServices.Internal.ClientProxy /t:code www.itron.com.ami.wsdl *.xsd /out:InternalServiceClientProxy.cs
