﻿// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Core.Graph;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math;
using OsmSharp.Routing.Core.Graph.DynamicGraph;

namespace OsmSharp.Routing.Core.Router
{
    /// <summary>
    /// Abstracts a data source of a router that is a dynamic graph with an extra lookup function.
    /// </summary>
    /// <typeparam name="EdgeData"></typeparam>
    public interface IBasicRouterDataSource<EdgeData> : IDynamicGraphReadOnly<EdgeData>
        where EdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Returns a list of edges inside or intersecting with the given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        KeyValuePair<uint, KeyValuePair<uint, EdgeData>>[] GetArcs(GeoCoordinateBox box);

        /// <summary>
        /// Returns the tags index.
        /// </summary>
        ITagsIndex TagsIndex
        {
            get;
        }
    }
}