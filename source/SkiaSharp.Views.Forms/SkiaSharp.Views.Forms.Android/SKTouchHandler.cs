﻿using System;
using Android.Views;

namespace SkiaSharp.Views.Forms
{
	internal class SKTouchHandler
	{
		private Action<SKTouchActionEventArgs> onTouchAction;
		private Func<float, float> scalePixels;

		public SKTouchHandler(Action<SKTouchActionEventArgs> onTouchAction, Func<float, float> scalePixels)
		{
			this.onTouchAction = onTouchAction;
			this.scalePixels = scalePixels;
		}

		public void Attach(View view)
		{
			view.Touch += OnTouch;
		}

		public void Detach(View view)
		{
			// clean the view
			if (view != null)
			{
				view.Touch -= OnTouch;
			}

			// remove references
			onTouchAction = null;
			scalePixels = null;
		}

		private void OnTouch(object sender, View.TouchEventArgs e)
		{
			if (onTouchAction == null || scalePixels == null)
				return;

			var evt = e.Event;
			var pointer = evt.ActionIndex;

			var id = evt.GetPointerId(pointer);
			var coords = new SKPoint(scalePixels(evt.GetX(pointer)), scalePixels(evt.GetY(pointer)));

			switch (evt.ActionMasked)
			{
				case MotionEventActions.Down:
				case MotionEventActions.PointerDown:
					{
						var args = new SKTouchActionEventArgs(id, SKTouchActionType.Pressed, coords);
						onTouchAction(args);
						e.Handled = args.Handled;
						break;
					}

				case MotionEventActions.Move:
					{
						var count = evt.PointerCount;
						for (pointer = 0; pointer < count; pointer++)
						{
							id = evt.GetPointerId(pointer);
							coords = new SKPoint(scalePixels(evt.GetX(pointer)), scalePixels(evt.GetY(pointer)));

							var args = new SKTouchActionEventArgs(id, SKTouchActionType.Moved, coords);
							onTouchAction(args);
							e.Handled = e.Handled || args.Handled;
						}
						break;
					}

				case MotionEventActions.Up:
				case MotionEventActions.PointerUp:
					{
						var args = new SKTouchActionEventArgs(id, SKTouchActionType.Released, coords);
						onTouchAction(args);
						e.Handled = args.Handled;
						break;
					}

				case MotionEventActions.Cancel:
					{
						var args = new SKTouchActionEventArgs(id, SKTouchActionType.Cancelled, coords);
						onTouchAction(args);
						e.Handled = args.Handled;
						break;
					}
			}
		}
	}
}
