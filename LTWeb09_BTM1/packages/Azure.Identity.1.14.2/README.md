# Azure Identity client library for .NET

The Azure Identity library provides [Microsoft Entra ID](https://learn.microsoft.com/entra/fundamentals/whatis) ([formerly Azure Active Directory](https://learn.microsoft.com/entra/fundamentals/new-name)) token authentication support across the Azure SDK. It provides a set of [`TokenCredential`](https://learn.microsoft.com/dotnet/api/azure.core.tokencredential?view=azure-dotnet) implementations that can be used to construct Azure SDK clients that support Microsoft Entra token authentication.

[Source code][source] | [Package (NuGet)][package] | [API reference documentation][identity_api_docs] | [Microsoft Entra ID documentation][entraid_doc]

## Getting started

### Install the package

Install the Azure Identity client library for .NET with NuGet:

```dotnetcli
dotnet add package Azure.Identity
```

### Prerequisites

* An [Azure subscription][azure_sub].
* The [Azure CLI][azure_cli] can also be useful for authenticating in a development environment, creating accounts, and managing account roles.

### Authenticate the client

When debugging and executing code locally, it's typical for a developer to use their own account for authenticating calls to Azure services. There are several developer tools that can be used to perform this authentication in your development environment.

#### Authenticate via Visual Studio

Developers using Visual Studio 2017 or later can authenticate a Microsoft Entra account through the IDE. Apps using `DefaultAzureCredential` or `VisualStudioCredential` can then use this account to authenticate calls in their app when running locally.

To authenticate in Visual Studio, select the **Tools** > **Options** menu to launch the **Options** dialog. Then navigate to the **Azure Service Authentication** options to sign in with your Microsoft Entra account.

![Visual Studio Account Selection][vs_login_image]

#### Authenticate via the Azure CLI

Developers coding outside of an IDE can also use the [Azure CLI][azure_cli] to authenticate. Apps using `DefaultAzureCredential` or `AzureCliCredential` can then use this account to authenticate calls in their app when running locally.

To authenticate with the Azure CLI, run the command `az login`. For users running on a system with a default web browser, the Azure CLI launches the browser to authenticate the user.

![Azure CLI Account Sign In][azure_cli_login_image]

For systems without a default web browser, the `az login` command uses the device code authentication flow. The user can also force the Azure CLI to use the device code flow rather than launching a browser by specifying the `--use-device-code` argument.

![Azure CLI Account Device Code Sign In][azure_cli_login_device_code_image]

#### Authenticate via the Azure Developer CLI

Developers coding outside of an IDE can also use the [Azure Developer CLI][azure_developer_cli] to authenticate. Apps using `DefaultAzureCredential` or `AzureDeveloperCliCredential` can then use this account to authenticate calls in their app when running locally.

To authenticate with the Azure Developer CLI, run the command `azd auth login`. For users running on a system with a default web browser, the Azure Developer CLI launches the browser to authenticate the user. For systems without a default web browser, the `azd auth login --use-device-code` command uses the device code authentication flow.

#### Authenticate via Azure PowerShell

Developers coding outside of an IDE can also use [Azure PowerShell][azure_powerShell] to authenticate. Apps using `DefaultAzureCredential` or `AzurePowerShellCredential` can then use this account to authenticate calls in their app when running locally.

To authenticate with Azure PowerShell, run the command `Connect-AzAccount`. For users running on a system with a default web browser and version 5.0.0 or later of Azure PowerShell, it launches the browser to authenticate the user. For systems without a default web browser, the `Connect-AzAccount` command uses the device code authentication flow. The user can also force Azure PowerShell to use the device code flow rather than launching a browser by specifying the `UseDeviceAuthentication` argument.

## Key concepts

### Credentials

A credential is a class that contains or can obtain the data needed for a service client to authenticate requests. Service clients across the Azure SDK accept credentials when they're constructed. Service clients use those credentials to authenticate requests to the service.

The Azure Identity library focuses on OAuth authentication with Microsoft Entra ID. It offers numerous credentials capable of acquiring a Microsoft Entra token to authenticate service requests. Each credential in this library is an implementation of the `TokenCredential` abstract class in [Azure.Core][azure_core_library], and any of them can be used to construct service clients capable of authenticating with a `TokenCredential`.

See [Credential classes](#credential-classes) for a complete listing of available credential types.

### DefaultAzureCredential

`DefaultAzureCredential` simplifies authentication while developing apps that deploy to Azure by combining credentials used in Azure hosting environments with credentials used in local development. For more information, see [DefaultAzureCredential overview][dac_overview].

#### Continuation policy

As of version 1.10.1, `DefaultAzureCredential` attempts to authenticate with all developer tool credentials until one succeeds, regardless of any errors previous developer tool credentials experienced. For example, a developer tool credential may attempt to get a token and fail, so `DefaultAzureCredential` will continue to the next credential in the flow. Deployed service credentials stop the flow with a thrown exception if they're able to attempt token retrieval but don't receive one. Prior to version 1.10.1, developer tool credentials would similarly stop the authentication flow if token retrieval failed.

This behavior allows for trying all of the developer tool credentials on your machine while having predictable deployed behavior.

## Examples

### Specify a user-assigned managed identity with `DefaultAzureCredential`

Many Azure hosts allow the assignment of a user-assigned managed identity. The following examples demonstrate configuring `DefaultAzureCredential` to authenticate a user-assigned managed identity when deployed to an Azure host. The sample code uses the credential to authenticate a `BlobClient` from the [Azure.Storage.Blobs][blobs_client_library] client library. It also demonstrates how you can specify a user-assigned managed identity either by a client ID or a resource ID.

#### Client ID

To use a client ID, take one of the following approaches:

1. Set the [DefaultAzureCredentialOptions.ManagedIdentityClientId](https://learn.microsoft.com/dotnet/api/azure.identity.defaultazurecredentialoptions.managedidentityclientid?view=azure-dotnet) property. For example:

```C# Snippet:UserAssignedManagedIdentityWithClientId
// When deployed to an Azure host, DefaultAzureCredential will authenticate the specified user-assigned managed identity.

string userAssignedClientId = "<your managed identity client ID>";
var credential = new DefaultAzureCredential(
    new DefaultAzureCredentialOptions
    {
        ManagedIdentityClientId = userAssignedClientId
    });

var blobClient = new BlobClient(
    new Uri("https://myaccount.blob.core.windows.net/mycontainer/myblob"),
    credential);
```

2. Set the `AZURE_CLIENT_ID` environment variable.

#### Resource ID

To use a resource ID, set the [DefaultAzureCredentialOptions.ManagedIdentityResourceId](https://learn.microsoft.com/dotnet/api/azure.identity.defaultazurecredentialoptions.managedidentityresourceid?view=azure-dotnet) property. The resource ID takes the form `/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ManagedIdentity/userAssignedIdentities/{identityName}`. Because resource IDs can be built by convention, they can be more convenient when there are a large number of user-assigned managed identities in your environment. For example:

```C# Snippet:UserAssignedManagedIdentityWithResourceId
string userAssignedResourceId = "<your managed identity resource ID>";
var credential = new DefaultAzureCredential(
    new DefaultAzureCredentialOptions
    {
        ManagedIdentityResourceId = new ResourceIdentifier(userAssignedResourceId)
    });

var blobClient = new BlobClient(
    new Uri("https://myaccount.blob.core.windows.net/mycontainer/myblob"),
    credential);
```

### Define a custom authentication flow with `ChainedTokenCredential`

While `DefaultAzureCredential` is generally the quickest way to authenticate apps for Azure, you can create a customized chain of credentials to be considered. `ChainedTokenCredential` enables users to combine multiple credential instances to define a customized chain of credentials. For more information, see [ChainedTokenCredential overview][ctc_overview].

## Managed identity support

[Managed identity authentication](https://learn.microsoft.com/entra/identity/managed-identities-azure-resources/overview) is supported either indirectly via `DefaultAzureCredential` or directly via `ManagedIdentityCredential` for the following Azure services:

* [Azure App Service and Azure Functions](https://learn.microsoft.com/azure/app-service/overview-managed-identity?tabs=dotnet)
* [Azure Arc](https://learn.microsoft.com/azure/azure-arc/servers/managed-identity-authentication)
* [Azure Cloud Shell](https://learn.microsoft.com/azure/cloud-shell/msi-authorization)
* [Azure Kubernetes Service](https://learn.microsoft.com/azure/aks/use-managed-identity)
* [Azure Service Fabric](https://learn.microsoft.com/azure/service-fabric/concepts-managed-identity)
* [Azure Virtual Machines](https://learn.microsoft.com/entra/identity/managed-identities-azure-resources/how-to-use-vm-token)
* [Azure Virtual Machines Scale Sets](https://learn.microsoft.com/entra/identity/managed-identities-azure-resources/qs-configure-powershell-windows-vmss)

As of version 1.8.0, `ManagedIdentityCredential` supports [token caching](#token-caching).

## Sovereign cloud configuration

By default, credentials authenticate to the Microsoft Entra endpoint for the Azure Public Cloud. To access resources in other clouds, such as Azure US Government or a private cloud, use one of the following solutions:

1. Configure credentials with the [AuthorityHost](https://learn.microsoft.com/dotnet/api/azure.identity.tokencredentialoptions.authorityhost?view=azure-dotnet#azure-identity-tokencredentialoptions-authorityhost) property. For example:

```C# Snippet:AuthenticatingWithAuthorityHost
var credential = new DefaultAzureCredential(
    new DefaultAzureCredentialOptions
    {
        AuthorityHost = AzureAuthorityHosts.AzureGovernment
    });
```

[AzureAuthorityHosts](https://learn.microsoft.com/dotnet/api/azure.identity.azureauthorityhosts?view=azure-dotnet) defines authorities for well-known clouds.

2. Set the `AZURE_AUTHORITY_HOST` environment variable to the appropriate authority host URL. For example, `https://login.microsoftonline.us/`. Note that this setting affects all credentials in the environment. Use the previous solution to set the authority host on a specific credential.

Not all credentials require this configuration. Credentials that authenticate through a developer tool, such as `AzureCliCredential`, use that tool's configuration.

## Credential classes

### Credential chains

|Credential | Usage | Reference|
|-|-|-|
|[`DefaultAzureCredential`][ref_DefaultAzureCredential]|Provides a simplified authentication experience to quickly start developing apps run in Azure.|[DefaultAzureCredential overview][dac_overview]|
|[`ChainedTokenCredential`][ref_ChainedTokenCredential]|Allows users to define custom authentication flows comprised of multiple credentials.|[ChainedTokenCredential overview][ctc_overview]|

### Authenticate Azure-hosted apps

|Credential | Usage | Reference|
|-|-|-|
|[`EnvironmentCredential`][ref_EnvironmentCredential]|Authenticates a service principal or user via credential information specified in [environment variables](#environment-variables).||
|[`ManagedIdentityCredential`][ref_ManagedIdentityCredential]|Authenticates the managed identity of an Azure resource.|[user-assigned managed identity][uami_doc]<br>[system-assigned managed identity][sami_doc]|
|[`WorkloadIdentityCredential`][ref_WorkloadIdentityCredential]|Supports [Microsoft Entra Workload ID](https://learn.microsoft.com/azure/aks/workload-identity-overview) on Kubernetes.||

### Authenticate service principals

|Credential | Usage | Reference|
|-|-|-|
|[`AzurePipelinesCredential`][ref_AzurePipelinesCredential]|Supports [Microsoft Entra Workload ID](https://learn.microsoft.com/azure/devops/pipelines/release/configure-workload-identity?view=azure-devops) on Azure Pipelines.| [example](https://aka.ms/azsdk/net/identity/azurepipelinescredential/usage)|
|[`ClientAssertionCredential`][ref_ClientAssertionCredential]|Authenticates a service principal using a signed client assertion.||
|[`ClientCertificateCredential`][ref_ClientCertificateCredential]|Authenticates a service principal using a certificate. | [Service principal authentication](https://learn.microsoft.com/entra/identity-platform/app-objects-and-service-principals)|
|[`ClientSecretCredential`][ref_ClientSecretCredential]|Authenticates a service principal using a secret. | [Service principal authentication](https://learn.microsoft.com/entra/identity-platform/app-objects-and-service-principals)|

### Authenticate users

|Credential | Usage | Reference|
|-|-|-|
|[`AuthorizationCodeCredential`][ref_AuthorizationCodeCredential]|Authenticates a user with a previously obtained authorization code. | [OAuth2 authorization code](https://learn.microsoft.com/entra/identity-platform/v2-oauth2-auth-code-flow)|
|[`DeviceCodeCredential`][ref_DeviceCodeCredential]|Interactively authenticates a user on devices with limited UI. | [Device code authentication](https://learn.microsoft.com/entra/identity-platform/v2-oauth2-device-code)|
|[`InteractiveBrowserCredential`][ref_InteractiveBrowserCredential]|Interactively authenticates a user with the default system browser. | [Interactive browser authentication](https://aka.ms/azsdk/net/identity/interactivebrowsercredential/usage)|
|[`OnBehalfOfCredential`][ref_OnBehalfOfCredential]|Propagates the delegated user identity and permissions through the request chain. | [On-behalf-of authentication](https://learn.microsoft.com/entra/identity-platform/v2-oauth2-on-behalf-of-flow)|

### Authenticate via development tools

|Credential | Usage | Reference|
|-|-|-|
|[`AzureCliCredential`][ref_AzureCliCredential]|Authenticates in a development environment with the Azure CLI. | [Azure CLI authentication](https://learn.microsoft.com/cli/azure/authenticate-azure-cli)|
|[`AzureDeveloperCliCredential`][ref_AzureDeveloperCliCredential]|Authenticates in a development environment with the Azure Developer CLI. | [Azure Developer CLI Reference](https://learn.microsoft.com/azure/developer/azure-developer-cli/reference)|
|[`AzurePowerShellCredential`][ref_AzurePowerShellCredential]|Authenticates in a development environment with the Azure PowerShell. | [Azure PowerShell authentication](https://learn.microsoft.com/powershell/azure/authenticate-azureps)|
|[`VisualStudioCredential`][ref_VisualStudioCredential]|Authenticates in a development environment with Visual Studio. | [Visual Studio configuration](https://learn.microsoft.com/dotnet/azure/configure-visual-studio)|

> __Note:__ All credential implementations in the Azure Identity library are threadsafe, and a single credential instance can be used by multiple service clients.

## Environment variables

[`DefaultAzureCredential`][ref_DefaultAzureCredential] and [`EnvironmentCredential`][ref_EnvironmentCredential] can be configured with environment variables. Each type of authentication requires values for specific variables. Configuration is attempted in the order in which these environment variables are listed. For example, if valuŒá o™ 
}‚ {€ o^ {‚ o &r.'p{‹ r(p(4 
}† ,r/(p(³  
(s }Œ (R +3{€ {† o” 
ry(p}ˆ {ˆ {† (³  
}‰ (S (W {‚ {Š Œá o™ 
(P Şa(‹ -ş(d ,(X +4{€ ,{€ o¥ 
}€ { ,{ o¥ 
} ş~ o´ Ü*AL     m  $   ‘             K   O  š  U        K   ¤  ï         ‚{ -{ oØ 
} { *{ƒ *&ş{ *{… *{ * 0 õ   ) ~ r§(p(H o  +
{– (r  
{– o#	 
,q~ r:)poª  +{– o$	 
~ rı)p	o ++(V &	Y	0î{– o#	 
,.~ rÆ*p{– o$	 
o +Ş~ r,poª  +Ş
,(w  
Ü~ rÔ,pş{’ o¨  +ş{’ Ş~ o´ Ü*        ™¹ 
      Ïæ     0 A   * ~ ra-p(H o  +
şQ s%	 
{€ o” &Ş~ o´ Ü*       4     0 ´   + ~ rö-p(H o  +
{€ o– (T Ş
,o0  
Üş{‘ -(P +(X Şc(‹ -ş} ş~ rf.po« ş{‘ -(d ş{‘ ,(X +ş} (U Ş~ o´ Ü*(   # 	, 
       :P W    ‘§     0 ç  , ~ r÷.p(H o  +
s: %{ oA { oS oN {… (n Ü %rk/p¢%{Œ ¢%r/p¢%{‹ ¢%r0p¢%{‹ ¢%rT0p¢%	¢%r…1p¢%	{‹ ¢%
r³1p¢%	¢%rÕ1p¢%{‹ ¢%r2p¢%{Œ ¢%r?2p¢(P 
o” 
-	oU 
&+Y oU 
&Ş9(‹ -ş(d ,o£ 
Ş(‹ -ş(d Ş Ş -{ oS oN Ü %r{2p¢%	¢%r—2p¢%{‹ ¢%rß2p¢%{Œ ¢%r#3p¢%	¢%ræ3p¢%	{‹ ¢%
r4p¢%{‹ ¢%r44p¢%{‹ ¢%rİ5p¢%{‹ ¢%r†6p¢%	¢(P 
o” 
s¿ %r 6po› 
%	o&	 
%oœ 
o^ o &oU 
&o 
¥$  ş$  oÛ  
} r¾6p{ rö6p{† (6 
}‡ {€ {‡ o” 
ry(p}ˆ {ˆ {€ oó 
(³  
}‰ o¼ 
ş}“ Ş0, o£ 
Ş(‹ -ş(d Ş Ü~ o´ Ü* A|                       ö   	   ÿ   9         ¹  
   Ã          *   Œ  ¶  $             Ä  Ú         0 Ï   - ~ r7p(H o  +
R|” (m  
~ rŒ7p~  o	  o  +{– (r  
{– o#	 
,8{– {– o$	 
Xoö 
~ r18p{– o$	 
o +Ş8{– oö 
R~ r(9poª  +Ş	,(w  
Ü~ o´ Ü*    H p¸ 
      ¬Â     0 4  . ~ rM:p(H o  +
~` 
ş{‘ :Z  8J  ~ rË:po« oı 
~ rV;poª  +o´ 
rë;p(Â 
9õ   o 	9Ö   	(Y 9¶   o¤ ~ rn<poª  +;oa 
2{o²  
~  {  	
	
(r  
~  {  o'	 
Ş
,	(w  
Ü,!o™ Şo(‹ -ş(d ŞY~ rû<po« +H~ r>po« +7~ rÏ>po« +&~ r®?po« +~` 
~ r{@po« o¤ 
:«şÿÿİ«   ~` 
((	 
,F{€ {‡ %-&{† o” 
{€ o^ o1 
1d{€ o^ { o› +L{€ {‰ o” 
{ Œ$  o™ 
{€ o^ o1 
3{€ o^ { o &~ o´ Ü*AL     Ğ      î              ş      	             l  ˆ  «       0 m  / ~ r:Ap(H o  +
(r  
ş{‘ - { o£ 
Ş	(‹ -ş	(d Ş Ş
,(w  
Ü(r  
ş{‘ -F{ƒ o] ,.(/ 
{„ o" 
{ o» 
Ş,()	 
Ü{ o» 
Ş,(w  
Ü(r  
ş{‘ -Mş{“ ,C	(R Ş

(‹ -ş
(d 	Ş 	,~  (N şs¢ o  Ş,(w  
Ü(r  
ş{‘ -V{‚ Œá o™ 
(W {‚ {Š Œá o™ 
(P ş} {• ,}• oq 
Ş,(w  
Üş{‘ ,(X İ¡   (‹ -ş(d ~  (N şs¢ o  Ş(‹ -ş(d Ş  { o£ 
Ş(‹ -ş(d Ş ş{‘ -şU sN 
{Š sı 
}• Ş~ o´ Ü*   A     ,      9              4   N   
          †                    ^   [   ¹              ë   	   ô           Ë   b   -            ?  k   ª             ß     û                              µ  Ë  •           J  `         0 õ  0 ~ r’Ap(H o  +
R9—   {– (r  
{– o#	 
,]{– o$	 
~ räApo +1{– Yoö 
+~ rÉBpo« 3!{– o*	 
&RŞ~ r`Cpo« Ş
	,(w  
Ü|” ( 
:ı   ~ rDpo« (r  
{€ o­ 
Ş(‹ -ş(d Ş ş}‘ Ş,(w  
Ü(+	 
	
	
(r  
ş{’ ,İ•   ş{ -o,	 
(-	 
2I~ r¶Dpo,	 
(-	 
ş{ o  +{• }• ,oq 
(X Ş.Ş
,	(w  
Ü(W 
8bÿÿÿ~ rEp{” o  +ş{’ Ş~ o´ Ü*   A|     (   „   ¬   
           æ      ó           İ   7               -     ®               Ğ  æ         0 B   1 ~ r<Fp(H o  +
{€ o‘ (T Ş,o0  
Ü~ o´ Ü*     " 	+ 
      5     0 g  2 ~ rÏFp(H o  +
(r  
{ oº 
9ö   { oº 
;ä   {€ o^ o1 
1H{€ {ˆ o” 
{€ o^ {‚ o› {€ oU 
&Ş	(‹ -ş	(d Ş ş{“ ,ş{ -u{€ Ü %r=Gp¢%{‹ ¢%rGp¢%{‹ ¢%r2p¢%{Œ ¢%r›Gp¢(P 
o” 
{€ oU 
&ŞT(‹ -ş(d Ş>Ş<ş}’ { o¥ 
{„ %-&+( 
Ü,(w  
Ü~ o´ Ü* A|      W   5   Œ                           "     *  &             6  P  
             D  Z         0 @  3 o¦ 

~B 
~B 
o· 
&o 
@î  rÇGpo 
(¬ 
9Ù  o.	 
=Í  8   o 
rëGp(¬ 
-$r|»p(¬ 
-prõGp(¬ 
:·   8$   ĞÊ  (‹  
o½ 
(
 
¥Ê  	ĞÊ  (‹  
	ŒÊ  (Ã 
,	Ş

(‹ -ş
(d Ş `8Ê    ĞÉ  (‹  
o½ 
(
 
¥É  ĞÉ  (‹  
ŒÉ  (Ã 
,Ş(‹ -ş(d Ş `+ro½ 
rÿGp(¬ 
-rHp(¬ 
-r9Hp(¬ 
-+
+4+.+(( +,ĞÈ  (‹  
ŒÈ  (Ã 
,`Ş,(d ~ rcHpo½ 
o 
o +İY  .o0	 
:dşÿÿ.~ r†Ipo  +İ.  o· 
-~ rƒJpo« İ  o 
3o 
rxKp(í 
,~ rˆKpo« İÜ   o· 
-~ rˆKpo« İ½   o 
.~ rˆKpo« İ   o½ 
s1	 
%o2	 
o· 
-~ rˆKpo« Şjo 
3o½ 
o3	 
Ş ~ rˆKpo« Ş:,o0  
Ü	s¢ Ş~ rˆKpo« Ş
,o0  
Ü*A|      “   :   Í            í   ;   (           X   `  ¸  ,   ,     ¶  M                  ,  3  
       (Š  
*’(Š  
} }Ÿ }  }¡ *{¡ *{ *{  *   0 Á   4 t¢ 
-8®   38£   { ,{ ,{ -{ ,+{ -7{ -/{Ÿ {Ÿ (¬ 
,{  {  (Â 
,+D+@{ { ow 
,+{Ÿ {Ÿ (¬ 
,{  {  (Â 
,+*   0 J   V  
{ ,{ o{ 

{  ,{Ÿ o{ 
{  o{ 
XX
+{Ÿ o{ 
X
*(Š  
*  0 .   5 
(c 
Ş&Ş %-&~4 %-&şd su %€4 *       
   0    6 ĞX  (‹  
r}Lp 4  “  (4	 

,Ho 
Ğ (‹  
( 
,1s5	 
ŒX    où 
&Ğ~ (‹  
o6	 
t~ *~ rµLpoš * 0 (   ™ (‹ 
,j*(7	 

 '  sõ  
(8	 
(9	 
*.(b €­ *0 Ë   7 Ğ:  (‹  
 4  “  %Ğ¥  (‹  
¢%Ğ¦  (‹  
¢(:	 

,Ks;	 
ş	 +(<	 
}=	 
ş¥  o>	 
&o?	 
(@	 
}=	 
ş>	 
sA	 
ŞCŞ&Ş ~ rØMpĞ:  (‹  
o 
Ğ¥  (‹  
o 
Ğ¦  (‹  
o 
o  +**       ‡‡   *.si €® *(Š  
*s– *(Š  
*   0 w   8 (¤ 
{¯ oC 
o% 
us  (Õ {° {± {² {³ (Ë {´ ,${´ o÷ 3{´ o {´ o Ş(Õ Ü*     hn     ®{¶ -sB	 
}¶ +{¶ oU 
}µ *R}µ {¶ oU 
*{µ *jQ{µ ,{¶ oT 
**0 -   Z  (Š  

 (C	 
-(Ü  
+ (D	 
}· }¸ *š(£  
rÛNp{· Œ$  {¸ Œã (ß  
*V(Š  
}ğ }ñ *r(¶  
}ò }ó }ô *
*
*
**s 
zs 
zs 
zs 
zs 
zs 
z0 ^   V  {ô ,NJ{ô i2B
+{ô ‘‘.}ô *X
{ô i2ÜJ{ô iXTJ{ô iYT}ô *î(‚ ( 1#{ò {ó o	 {ó o &*  0 ‰   9 (‚ ( (/ 

1#{ò {ó o	 {ó o 
%-&(À 
Ş9&{ò {” o  ş&{ò {” o  ş&{ò {” o  ş*   (     6N Á     6a Â     6t Ã  0 E       -r„}p(l z/r’}p(n z/r¯`p(n z Öi1(9 zŞ&(9 z*       - = ™  Â(E	 
}õ }ö }÷ {ö / ÿÿÿ}ö *v{ö 1{÷ {ö şş**2{õ oF	 
*2{õ oG	 
*2{õ oH	 
*¦{÷ {ö /{õ oI	 
{÷ X}÷ *ú( {ö {÷ Y(9 
1{õ oJ	 
{÷ X}÷ *¾{÷ {ö /{÷ X}÷ {õ oK	 
*(À 
* 0 D       ( {ö {÷ Y(9 
1{÷ X}÷ {õ oL	 
*(À 
*6o® 
(M	 
*  0 E       -r„}p(l z/r’}p(n z/r¯`p(n z Öi1(9 zŞ&(9 z*       - = ™  .s €ø *(Š  
*Ft( {p	 o> *(Š  
*  0 ,   : s“ 
}ü }û ([ 
ş” sÃ 
o\ 
&*(Š  
*0 \       {ü {ú {p	 o< {ü {ú où {û o. 
Ş'{ü {ú où {ü {ú {p	 o> Ü*   ' 4 '    (Š  
*F{ı ş{Õ ş*(Š  
*  0 ^       o` 
,({ş {ÿ oa 
ob 
(	 oa 
ob 
z{ÿ o¦ Ş{  ,{ş {” {p	 o> Ü*       ??     (Š  
*0 V   ö t=  
{ { { { { { { { {	 {
 { { (’	 &*Nt=  { o˜	 *(Š  
*~{ { { { (™	 *(Š  
*  0 F       { { } { { }Š { { }‹ { { }Œ *  0 x  ; { 
{ 6((} 
    o~ 
} { st }  9£   ;S  } }     { 1{ X{ 1{ { Y{¥ 9„   { {& { oN	 
oO	 
(P	 
(Q	 
-A%
} }  | (
 +İ“  {  |  şV %
} (S	 
	} +{ {& { o  
} { -İ   {¥ ,|{ { { oT	 
oU	 
(V	 
(W	 
-A%
} }! | ( +İÜ   {! |! şÛ  %
} (X	 
+{ { { oÌ  
{ { X} { >Wşÿÿ{ { ?FşÿÿŞ/{ ,{ o0  
Ü} (} 
{ o„ 
Ş ş} } | (Y	 
Şş} } | (Z	 
*A4     ;   Ğ                  /  =        6| ([	 
*  0 W  < {" 
{% 6U(\	 
    o]	 
}+ {$ %-&s  
}$ {& st {$ s^	 
{' sƒ },  E   ^       „  5  {( 9ˆ   {¥ ,p{,  ÿş  oK	 
oU	 
(V	 
(W	 
-?%
}" }. |# ( +İv  {. |. şÛ  %
}" (X	 
+{,  ÿş  oI	 
}- }/ {¥ 9"  {) 9   {* {( {+     o_	 
oO	 
(P	 
(Q	 
-A%
}" }0 |# ( +İÃ  {0 |0 şV %
}" (S	 
}/ 8Ø   {* {( {+     o`	 
oO	 
(P	 
(Q	 
-A%
}" }0 |# ( +İ6  {0 |0 şV %
}" (S	 
}/ +N{) ,${* {( {+     o­ 
}/ +"{* {( {+     o· 
}/ {/ 9¾   {¥ ,{{, {+ {/ oL	 
oU	 
(V	 
		(W	 
-A%
}" 	}. |# 	( +İP  {. 	|. şÛ  %
}" 	(X	 
+{, {+ {/ oJ	 
{- {/ X}- {, o„ 9µıÿÿ{¥ ,n{, oH	 
oU	 
(V	 


(W	 
-A%
}" 
}. |# 
( +İŸ   {. 
|. şÛ  %
}" 
(X	 
+{, oG	 
Ş/{, ,{, o0  
Ü}, (\	 
{+ oa	 
Ş ş}" }+ |# (Y	 
Şş}" }+ |# (Z	 
* A4     h   ‚  ê                          6|# ([	 
*  0 j  = {1 
{5 9-  ;Ó  {3 -{4 ob	 
{6 st {4 s^	 
{7 sƒ }9 sı 
	oş 
	oÿ 
{¥ ,	oc	 
{9 	(  
}: {8 {) od	 
:Ú   {8 {) o· 
&8Ä   {8 {) o 
