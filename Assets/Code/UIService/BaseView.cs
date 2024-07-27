using System;
using UnityEngine;

namespace MyProject.ReactionBurst.UI
{
    /// <summary>
    /// Base class for all View: only visualize data and receive user input
    /// </summary>
    public class BaseView: MonoBehaviour
    {
        public event Action ClosedCompletely = delegate { };
        public event Action Hiden = delegate {  };


        /// <summary>
        /// Fill View default (first) data
        /// </summary>
        /// <param name="data">Default data</param>
        /// <typeparam name="T">Type of Data</typeparam>
        public virtual void Prepare<T>(T data) where T : BaseModel
        {

        }

        /// <summary>
        /// Update View field by dataName
        /// </summary>
        /// <param name="dataName">Name of data</param>
        /// <param name="value">New value</param>
        public virtual void UpdateData(string dataName, object value)
        {
            
        }

        /// <summary>
        /// Callback for Presenter for Close this View
        /// </summary>
        protected virtual void OnCloseButtonPressed()
        {
            ClosedCompletely.Invoke();
        }

        public virtual void OnHideButtonPressed()
        {
            Hiden.Invoke();
        }
    }
}