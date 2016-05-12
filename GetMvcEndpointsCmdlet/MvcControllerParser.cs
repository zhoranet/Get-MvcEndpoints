using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MvcEndpointsDiscovery
{
    internal static class MvcControllerParser
    {
        public static IEnumerable<MvcEndpointInfo> GetEndpointsCollection(SyntaxNode root)
        {
            var uniqueMethodNames = new HashSet<string>();

            var methods =
                from m in root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                where m.Modifiers.ToString().Contains("public")
                select m;
            
            return methods.Select(m => new MvcEndpointInfo
            {
                EndpointName = VerifyUniqueName(uniqueMethodNames, m.Identifier.ValueText),
                HttpMethod = GetHttpMethodName(m),
                Route = GetRouteAttributeValue(m)
            });
        }

        internal static string VerifyUniqueName(HashSet<string> uniqueNames, string newName)
        {
            string uniqueName = newName;

            if(uniqueNames == null)
                throw new ArgumentNullException("uniqueNames");

            if (uniqueNames.Any())
            {
                var lastName =
                    uniqueNames.Where(x => x.StartsWith(newName))
                        .OrderByDescending(x => x)
                        .FirstOrDefault();

                if (lastName != null)
                {
                    var suffix = lastName.Replace(newName, null);

                    if (String.IsNullOrWhiteSpace(suffix))
                        suffix = "0";

                    int index;
                    uniqueName = newName + (Int32.TryParse(suffix, out index) ? (++index).ToString() : "1");
                }
            }

            uniqueNames.Add(uniqueName);

            return uniqueName;
        }

        public static string GetRouteAttributeValue(MethodDeclarationSyntax method)
        {
            const string routePrexix = "Route";

            return GetAttributes(method).Where(x => x.StartsWith(routePrexix))
                .Select(y => y.Remove(0, routePrexix.Length).Trim('(', ')').Trim('\"')).FirstOrDefault();
        }

        public static string GetHttpMethodName(MethodDeclarationSyntax method, string defaultName = "GET")
        {
            const string httpAttributeNamePrefix = "Http";

            return GetAttributes(method).Where(x => x.StartsWith(httpAttributeNamePrefix))
                .Select(y => y.Remove(0, httpAttributeNamePrefix.Length)).FirstOrDefault() ?? defaultName;
        }

        public static IEnumerable<string> GetAttributes(MethodDeclarationSyntax method)
        {
            return method.AttributeLists.SelectMany(x => x.Attributes, (y, p) => y.ToString().Trim('[', ']'));
        }

        public static SyntaxNode GetControllerRoot(string filePath, string[] baseClasses)
        {
            var text = File.ReadAllText(filePath);

            var tree = CSharpSyntaxTree.ParseText(text);
            var root = tree.GetRoot();

            var isValid = IsValidController(root, baseClasses);

            return isValid ? root : null;
        }

        public static bool IsValidController(SyntaxNode root, string[] baseClasses)
        {
            bool isValid = false;
            try
            {
                var descendant = root.DescendantNodes().OfType<BaseTypeSyntax>().First();
                isValid = (baseClasses.Any(baseClass => descendant.ToString().Contains(baseClass)));

                if (isValid)
                {
                    var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
                    isValid = !classDeclaration.Modifiers.Any(x => x.ToString() == "static" || x.ToString() == "abstract");
                }
                
            }
            catch (InvalidOperationException)
            {
            }

            return isValid;
        }
    }
}