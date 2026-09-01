using System;
using System.Collections.Generic;
using System.Text;

namespace EasyFramework.UnityRoslyn
{
    internal static class Helper
    {

        public static string GeneratorNamespaceBodyString(string body, string namespaceName)
        {
            if (namespaceName == string.Empty) return body;

            return $@"
namespace {namespaceName}
{{
{body}
}}";
        }

    }
}
