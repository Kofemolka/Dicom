using System;
using System.Collections.Generic;

namespace Model
{
    public delegate void LabelDataChangedEvent();

    public interface ILabelMap
    {
        System.Drawing.Color Color { get; set; }

#if DEBUG
        void AddDebugPoint(Point3D point);
        IEnumerable<Point3D> GetDebugProjection(Axis axis, int index);
#endif
        void Add(Point3D point);
        void Add(IEnumerable<Point3D> points);
        void AddCenter(Point3D point);
                
        void Reset();

        IEnumerable<Point3D> GetProjection(Axis axis, int index);
        IEnumerable<Point3D> GetCenters();
        IEnumerable<Point3D> GetAll();

        void FireUpdate();

        event LabelDataChangedEvent LabelDataChanged;
    }

    public class LabelMap : ILabelMap
    {
        private readonly List<Point3D> _marks = new List<Point3D>();
        private readonly List<Point3D> _centers = new List<Point3D>();

#if DEBUG
        private readonly List<Point3D> _debugs = new List<Point3D>();
        
        public void AddDebugPoint(Point3D point)
        {
            lock (_debugs)
            {
                _debugs.Add(point);
            }
        }

        public IEnumerable<Point3D> GetDebugProjection(Axis axis, int index)
        {
            foreach (var mark in _debugs)
            {
                switch (axis)
                {
                    case Axis.X:
                        if (mark.X == index)
                        {
                            yield return mark;
                        }
                        break;

                    case Axis.Y:
                        if (mark.Y == index)
                        {
                            yield return mark;
                        }
                        break;

                    case Axis.Z:
                        if (mark.Z == index)
                        {
                            yield return mark;
                        }
                        break;
                }
            }
        }
#endif
        public System.Drawing.Color Color { get; set; }

        public void FireUpdate()
        {
            LabelDataChanged?.Invoke();
        }

        public void Add(Point3D point)
        {
            lock (_marks)
            {
                _marks.Add(point);
            }
        }

        public void Add(IEnumerable<Point3D> points)
        {
            lock (_marks)
            {
                _marks.AddRange(points);
            }
        }

        public void AddCenter(Point3D point)
        {
            lock (_centers)
            {
                _centers.Add(point);
            }
        }

        public IEnumerable<Point3D> GetProjection(Axis axis, int index)
        {
            foreach (var mark in _marks)
            {
                switch (axis)
                {
                    case Axis.X:
                        if (mark.X == index)
                        {
                            yield return mark;
                        }
                        break;

                    case Axis.Y:
                        if (mark.Y == index)
                        {
                            yield return mark;
                        }
                        break;

                    case Axis.Z:
                        if (mark.Z == index)
                        {
                            yield return mark;
                        }
                        break;
                }
            }
        }

        public IEnumerable<Point3D> GetAll()
        {
            foreach (var mark in _marks)
            {
                yield return mark;
            }
        }

        public IEnumerable<Point3D> GetCenters()
        {
            return _centers;
        }

        public void Reset()
        {
            _marks.Clear();
            _centers.Clear();

#if DEBUG
            _debugs.Clear();
#endif
            FireUpdate();
        }

        public event LabelDataChangedEvent LabelDataChanged;
    }
}
