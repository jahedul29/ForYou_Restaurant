#pragma checksum "D:\Project\Brugen_Patel\ForYou\ForYou\Views\Shared\Components\UserName\Default.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2e09f67f4b611a5a2dfe3bd5214b2f8533d2146f"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared_Components_UserName_Default), @"mvc.1.0.view", @"/Views/Shared/Components/UserName/Default.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\Project\Brugen_Patel\ForYou\ForYou\Views\_ViewImports.cshtml"
using ForYou;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Project\Brugen_Patel\ForYou\ForYou\Views\_ViewImports.cshtml"
using ForYou.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2e09f67f4b611a5a2dfe3bd5214b2f8533d2146f", @"/Views/Shared/Components/UserName/Default.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"461b6b04e37892e3c46774148cdc121277e751b2", @"/Views/_ViewImports.cshtml")]
    public class Views_Shared_Components_UserName_Default : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ApplicationUser>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("Hi! ");
#nullable restore
#line 2 "D:\Project\Brugen_Patel\ForYou\ForYou\Views\Shared\Components\UserName\Default.cshtml"
Write(Model.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("&nbsp;<i class=\"fas fa-user\"></i>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ApplicationUser> Html { get; private set; }
    }
}
#pragma warning restore 1591
