﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.DynamicGraph;

namespace Osm.Routing.CH.PreProcessing.Ordering
{
    /// <summary>
    /// The edge difference calculator.
    /// </summary>
    public class EdgeDifferenceContractedSearchSpace : INodeWeightCalculator
    {
        /// <summary>
        /// Holds the graph.
        /// </summary>
        private INodeWitnessCalculator _witness_calculator;

        /// <summary>
        /// Holds the data.
        /// </summary>
        private IDynamicGraph<CHEdgeData> _data;

        /// <summary>
        /// Holds the contracted count.
        /// </summary>
        private Dictionary<uint, short> _contraction_count;

        /// <summary>
        /// Holds the depth.
        /// </summary>
        private Dictionary<long, long> _depth;

        /// <summary>
        /// Creates a new edge difference calculator.
        /// </summary>
        /// <param name="graph"></param>
        public EdgeDifferenceContractedSearchSpace(IDynamicGraph<CHEdgeData> data, INodeWitnessCalculator witness_calculator)
        {
            _data = data;
            _witness_calculator = witness_calculator;
            _contraction_count = new Dictionary<uint, short>();
            _depth = new Dictionary<long, long>();
        }

        /// <summary>
        /// Calculates the edge-difference if u would be contracted.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public float Calculate(uint vertex)
        {
            // simulate the construction of new edges.
            int new_edges = 0;
            int removed = 0;

            short contracted = 0;
            _contraction_count.TryGetValue(vertex, out contracted);

            // get the neighbours.
            KeyValuePair<uint, CHEdgeData>[] neighbours = _data.GetArcs(vertex);

            foreach (KeyValuePair<uint, CHEdgeData> from in neighbours)
            { // loop over all incoming neighbours
                if (!from.Value.Backward) { continue; }

                foreach (KeyValuePair<uint, CHEdgeData> to in neighbours)
                { // loop over all outgoing neighbours
                    if (!to.Value.Forward) { continue; }

                    if (to.Key != from.Key)
                    { // the neighbours point to different vertices.
                        // a new edge is needed.
                        if (!_witness_calculator.Exists(from.Key, to.Key, vertex,
                            from.Value.Weight + to.Value.Weight))
                        { // no witness exists.
                            new_edges++;
                        }
                    }
                }

                // count the edges.
                if (from.Value.Forward)
                {
                    removed++;
                }
                if (from.Value.Backward)
                {
                    removed++;
                }
            }

            // get the depth.                    
            long depth = 0;
            _depth.TryGetValue(vertex, out depth);
            //return (new_edges - removed) + (contracted * 3) + depth;
            return (new_edges - removed) + depth;
        }

        /// <summary>
        /// Notifies this calculator that the vertex was contracted.
        /// </summary>
        /// <param name="vertex_id"></param>
        public void NotifyContracted(uint vertex)
        {
            // removes the contractions count.
            _contraction_count.Remove(vertex);

            // loop over all neighbours.
            KeyValuePair<uint, CHEdgeData>[] neighbours = _data.GetArcs(vertex);
            foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours)
            {
                short count;
                if (!_contraction_count.TryGetValue(neighbour.Key, out count))
                {
                    _contraction_count[neighbour.Key] = 1;
                }
                else
                {
                    _contraction_count[neighbour.Key] = count++;
                }
            }

            long vertex_depth = 0;
            _depth.TryGetValue(vertex, out vertex_depth);
            _depth.Remove(vertex);
            vertex_depth++;

            // store the depth.
            foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours)
            {
                if (!_contraction_count.ContainsKey(neighbour.Key))
                {
                    long depth = 0;
                    _depth.TryGetValue(neighbour.Key, out depth);
                    if (vertex_depth > depth)
                    {
                        _depth[neighbour.Key] = depth;
                    }
                    else
                    {
                        _depth[neighbour.Key] = vertex_depth;
                    }
                }
            }
        }
    }
}