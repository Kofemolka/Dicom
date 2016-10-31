using System.Collections.Generic;

namespace Model
{
    public delegate void LabelDataChangedEvent();

    public interface ILabelMap
    {
        void Add(Point3D point);
        void Add(IEnumerable<Point3D> points);
        IEnumerable<Point3D> GetProjection(Axis axis, int index);
        void Reset();

        IEnumerable<Point3D> GetAll();

        void FireUpdate();

        event LabelDataChangedEvent LabelDataChanged;
    }

    public class LabelMap : ILabelMap
    {
        private readonly List<Point3D> _marks = new List<Point3D>();

        public void FireUpdate()
        {
            LabelDataChanged?.Invoke();
        }

        public void Add(Point3D point)
        {
            _marks.Add(point);
        }

        public void Add(IEnumerable<Point3D> points)
        {
            _marks.AddRange(points);
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

        public void Reset()
        {
            _marks.Clear();

            FireUpdate();
        }

        public event LabelDataChangedEvent LabelDataChanged;
    }
}
