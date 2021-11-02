using System.Collections.Generic;
using System.IO;
using System.Linq;
using Community.VisualStudio.Toolkit;

namespace VsixNamespace
{
    public static class SolutionHelper
    {
        public static string ProjectItemKindFolder = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";
        public static string ProjectItemKindFile = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";

        public static Community.VisualStudio.Toolkit.PhysicalFolder? FindFolder(
            this SolutionItem root,
            string fullPath
            )
        {
            if (root.Type == SolutionItemType.PhysicalFolder && root is Community.VisualStudio.Toolkit.PhysicalFolder)
            {
                if (!string.IsNullOrEmpty(root.FullPath))
                {
                    if (root.FullPath == fullPath)
                    {
                        return (Community.VisualStudio.Toolkit.PhysicalFolder)root;
                    }
                }
            }

            foreach(var child in root.Children)
            {
                var result = FindFolder(child, fullPath);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static void FindFolders(
            this SolutionItem root,
            ref Dictionary<string, SolutionItem> folders,
            string[] skipFolderNames
            )
        {
            if (root.Type == SolutionItemType.PhysicalFolder)
            {
                if (!string.IsNullOrEmpty(root.FullPath))
                {
                    if (skipFolderNames.Contains(new DirectoryInfo(root.FullPath).Name))
                    {
                        return;
                    }

                    folders[root.FullPath] = root;
                }
            }

            foreach (var child in root.Children)
            {
                FindFolders(child, ref folders, skipFolderNames);
            }
        }

        public static Project FindProject(
            this SolutionItem root,
            string projectName
            )
        {
            if (root.Type == SolutionItemType.Solution || root.Type == SolutionItemType.SolutionFolder)
            {
                foreach (var child in root.Children)
                {
                    if (child?.Type == SolutionItemType.Project && child.Name == projectName)
                    {
                        return (Project)child;
                    }

                    var result = FindProject(child, projectName);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }
    }
}
