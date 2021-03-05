// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.PackageManagement.UI
{
    public class PackageManagerViewModel
    {
        private ObservableCollection<PackageItemViewModel> _packageItemViewModels;

        public PackageListViewModel PackageListViewModel { get; set; }

        public PackageManagerViewModel()
        {
            _packageItemViewModels = new ObservableCollection<PackageItemViewModel>();

            PackageListViewModel = new PackageListViewModel(_packageItemViewModels);
        }
    }
}
