﻿using System;
using EnvDTE;
using EnvDTE80;

namespace VsixNamespace
{
    public static class DTEHelper
    {
        public static bool TryGetSelectedProjectFolder(
            this DTE2 dte,
            out EnvDTE.ProjectItem? envProjectItem
            )
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var uih = dte.ToolWindows.SolutionExplorer;
            var selectedItems = (Array)uih.SelectedItems;
            if (selectedItems.Length != 1)
            {
                envProjectItem = null;
                return false;
            }

            foreach (UIHierarchyItem selectedItem in selectedItems)
            {
                if (!(selectedItem.Object is EnvDTE.ProjectItem projectItem))
                {
                    envProjectItem = null;
                    return false;
                }

                if (projectItem.Kind != SolutionHelper.ProjectItemKindFolder)
                {
                    envProjectItem = null;
                    return false;
                }

                envProjectItem = projectItem;
                return true;
            }

            envProjectItem = null;
            return false;
        }

        public static bool TryGetSelectedProject(
            this DTE2 dte,
            out EnvDTE.Project? envProject
            )
        {
            var uih = dte.ToolWindows.SolutionExplorer;
            var selectedItems = (Array)uih.SelectedItems;
            if (selectedItems.Length != 1)
            {
                envProject = null;
                return false;
            }

            foreach (UIHierarchyItem selectedItem in selectedItems)
            {
                if (!(selectedItem.Object is EnvDTE.Project project))
                {
                    envProject = null;
                    return false;
                }

                envProject = project;
                return true;
            }

            envProject = null;
            return false;
        }

    }
}
