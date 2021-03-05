// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NuGet.PackageManagement.UI
{
    /// <summary>
    /// Represents a list of packages containing <see cref="PackageItemViewModel"/> objects.
    /// </summary>
    public class PackageListViewModel
    {
        public ObservableCollection<PackageItemViewModel> Collection { get; private set; }
        public ICollectionView CollectionView
        {
            get
            {
                return CollectionViewSource.GetDefaultView(Collection);
            }
        }

        private PackageListViewModel()
        { }

        public PackageListViewModel(ObservableCollection<PackageItemViewModel> collection)
        {
            Collection = collection;
        }
    }
}
