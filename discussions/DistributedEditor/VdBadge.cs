﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Media;
using Discussions;
using System.Windows.Media.Effects;
using LoginEngine;
using Discussions.RTModel.Model;

namespace DistributedEditor
{
    class VdBadge : VdBaseShape, IVdShape, LinkableHost
    {
        Badge4 _badge;       
        Canvas scene;
        int _argPtId;
        ClientClusterable _endpoint;

        double _left;
        double _top;
  
        public VdBadge(System.IO.BinaryReader r, int owner, int shapeId, int argPtId):
            base(owner, shapeId)
        {
            _argPtId = argPtId;
            _endpoint = new ClientClusterable(shapeId, boundsProvider);
            init(DaoUtils.UserIdToColor(owner));
        }

        public VdBadge(double x, double y, int owner, int shapeId, int argPtId):
            base(owner, shapeId)
        {
            _argPtId = argPtId;
            _endpoint = new ClientClusterable(shapeId, boundsProvider);
            init(DaoUtils.UserIdToColor(owner));
            setBadgePos(x, y);    
        }

        void init(Color c)
        {    
            _badge = new Badge4();
            _badge.Tag = this;
            RecontextBadge();
        }

        public ShapeZ ShapeZLevel()
        {
            return ShapeZ.BADGE_Z_LEVEL; 
        }

        public void SetZIndex(int z)
        {
            Canvas.SetZIndex(_badge, z);
        }

        public void RecontextBadge()
        {
            _badge.DataContext = null;
            _badge.DataContext = DbCtx.Get().ArgPoint.FirstOrDefault(ap => ap.Id == _argPtId);
        }

        public UIElement UnderlyingControl()
        {
            return _badge;
        }

        override public void ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            TouchManip = true;
            base.ManipulationStarting(sender, e);            
        }

        public void ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Manipulation(e);
            e.Handled = true;
        }

        void Manipulation(ManipulationDeltaEventArgs e)
        {
            var mt = new MatrixTransform(ShapeUtils.GetTransform(e));
            var newTopLeft = mt.Transform(getBadgePos());
            setBadgePos(newTopLeft.X, newTopLeft.Y);                 
        }
        
        //public only for user by cluster
        public Point getBadgePos()
        {
            return new Point(_left, _top);            
        }

        //public only for user by cluster
        public void setBadgePos(double x, double y)
        {
            _left = x;
            _top  = y;

            Canvas.SetLeft(_badge, _left);
            Canvas.SetTop(_badge, _top);

            GetLinkable().InvalidateLinks();
        }

        public override void SetCursor(Cursor c)
        {
            base.SetCursor(c);
            _badge.usrCursor.DataContext = c;
        }

        public override void UnsetCursor()
        {
            base.UnsetCursor();
            _badge.usrCursor.DataContext = null;
        }

        public override void RemoveFocus()
        {
            base.RemoveFocus();          
        }

        public override void SetFocus()
        {
            base.SetFocus();          
        }

        public void AttachToCanvas(Canvas c)
        {
            scene = c;

            if (!c.Children.Contains(_badge))
                c.Children.Add(_badge);
        }

        public void DetachFromCanvas(Canvas c)
        {
            scene = null;

            c.Children.Remove(_badge);
        }

        public VdShapeType ShapeCode()
        {
            return VdShapeType.Badge;
        }

        public ShapeState GetState(int topicId)
        {
            var topLeft = getBadgePos();
            return new ShapeState(  ShapeCode(), 
                                    InitialOwner(),
                                    Id(), 
                                    null,
                                    null,
                                    new double[] { topLeft.X, topLeft.Y },
                                    topicId);
        }

        public void ApplyState(ShapeState st)
        {
            setBadgePos(st.doubles[0], st.doubles[1]);
        }

        public void StartManip(Point p, object sender)
        {
            CurrentPoint = p;

            SetFocus();         
        }

        public PointApplyResult ApplyCurrentPoint(Point p)
        {
            if (TouchManip)
                return PointApplyResult.None;

            var res = PointApplyResult.None;

            var pos = getBadgePos();
            var dx = p.X - CurrentPoint.X;
            var dy = p.Y - CurrentPoint.Y;
            if (dx != 0.0 || dy != 0.0)
                res = PointApplyResult.Move;
            _left = pos.X + dx;
            _top  = pos.Y + dy;
            setBadgePos(_left,_top);
       
            CurrentPoint = p;

            return res;
        }     

        public void ScaleInPlace(bool plus)
        {            
        }

        public double distToFigure(Point from)
        {
            var right  = _left + _badge.ActualWidth;
            var bottom = _top  + _badge.ActualHeight;

            var d1 = ShapeUtils.Dist(new Point(_left, _top), from);
            var d2 = ShapeUtils.Dist(new Point(_left, bottom), from);
            var d3 = ShapeUtils.Dist(new Point(right, bottom), from);
            var d4 = ShapeUtils.Dist(new Point(right, _top), from); 

            return ShapeUtils.Min(d1, d2, d3, d4);
        }

        public void MoveBy(double dx, double dy)
        {
            throw new NotImplementedException("do we need this now?");           
        }
       
        public ClientLinkable GetLinkable()
        {
            return _endpoint;            
        }

        public ClientClusterable GetClusterable()
        {
            return _endpoint;
        }

        Rect boundsProvider()
        {
            if (_badge.ActualWidth < 1)
                return new Rect(_left, _top, 385, 240);
            
            return new Rect(_left, _top, _badge.ActualWidth, _badge.ActualHeight);
        }
    }
}
