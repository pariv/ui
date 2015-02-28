﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using NUnit.Framework;
using OsmSharp.Collections;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams;
using OsmSharp.Routing.Vehicles;
using System.Collections.Generic;
using System.Reflection;

namespace OsmSharp.Test.Unittests.Routing
{
    /// <summary>
    /// Tests the routing against a reference implementation.
    /// </summary>
    [TestFixture]
    public class ComparisonTests : RoutingComparisonTestsBase
    {
        /// <summary>
        /// Holds the data.
        /// </summary>
        private Dictionary<string, RouterDataSource<Edge>> _data = null;

        /// <summary>
        /// Returns a new router.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="embeddedName"></param>
        /// <param name="contract"></param>
        /// <returns></returns>
        public override Router BuildRouter(IOsmRoutingInterpreter interpreter, string embeddedName, bool contract)
        {
            if (_data == null)
            {
                _data = new Dictionary<string, RouterDataSource<Edge>>();
            }
            RouterDataSource<Edge> data = null;
            if (!_data.TryGetValue(embeddedName, out data))
            {
                var tagsIndex = new TagsTableCollectionIndex();

                // do the data processing.
                data = new RouterDataSource<Edge>(new Graph<Edge>(), tagsIndex);
                var targetData = new GraphOsmStreamTarget(
                    data, interpreter, tagsIndex, new Vehicle[] { Vehicle.Car }, false);
                var dataProcessorSource = new XmlOsmStreamSource(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(
                    "OsmSharp.Test.Unittests.{0}", embeddedName)));
                var sorter = new OsmStreamFilterSort();
                sorter.RegisterSource(dataProcessorSource);
                targetData.RegisterSource(sorter);
                targetData.Pull();

                _data[embeddedName] = data;
            }
            return Router.CreateFrom(data, new Dykstra(), interpreter);
        }

        /// <summary>
        /// Compares all routes possible against a reference implementation.
        /// </summary>
        [Test]
        public void TestAgainstReference()
        {
            this.TestCompareAll("test_network.osm", true);
        }

        /// <summary>
        /// Compares all routes possible against a reference implementation.
        /// </summary>
        [Test]
        public void TestAgainstReferenceBig()
        {
            this.TestCompareAll("test_network_big.osm", true);
        }

        /// <summary>
        /// Compares all routes possible against a reference implementation.
        /// </summary>
        [Test]
        public void TestOneWayAgainstReference()
        {
            this.TestCompareAll("test_network_oneway.osm", true);
        }

        /// <summary>
        /// Compares all routes possible against a reference implementation.
        /// </summary>
        [Test]
        public void TestRegression1()
        {
            this.TestCompareAll("test_routing_regression1.osm", true);
        }

        /// <summary>
        /// Tests one failing route specifically again.
        /// </summary>
        [Test]
        public void TestRegression2Regression1()
        {
            this.TestCompareOne("test_routing_regression2.osm", false, new OsmSharp.Math.Geo.GeoCoordinate(51.0219654, 3.9911377),
                new OsmSharp.Math.Geo.GeoCoordinate(51.0206158, 3.9932989));
        }

        /// <summary>
        /// Tests one failing route specifically again.
        /// </summary>
        [Test]
        public void TestRegression2Regression2()
        {
            this.TestCompareOne("test_routing_regression2.osm", false, new OsmSharp.Math.Geo.GeoCoordinate(51.0204852, 3.993617),
                new OsmSharp.Math.Geo.GeoCoordinate(51.0219591301773, 3.99107989102905));
        }

        /// <summary>
        /// Compares all routes possible against a reference implementation.
        /// </summary>
        [Test]
        public void TestRegression2()
        {
            this.TestCompareAll("test_routing_regression2.osm", true);
        }

        /// <summary>
        /// Compares all routes possible against a reference implementation.
        /// </summary>
        [Test]
        public void TestBig()
        {
            this.TestCompareAll("test_network_big.osm", true);
        }

        /// <summary>
        /// Compares all routes possible against a reference implementation.
        /// </summary>
        [Test]
        public void TestAgainstReferenceRealNetwork()
        {
            this.TestCompareAll("test_network_real1.osm", true);
        }
    }
}