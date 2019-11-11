namespace Blackcat.Configuration
{
    public enum SaveMode
    {
        /// <summary>
        /// Auto saving changes when application's exiting. Currently we only support for WinForm platform.
        /// </summary>
        OnExit,

        /// <summary>
        /// Auto saving changes when any setting changed immediately. We only support for configuration classes
        /// inherit from <see cref="AutoNotifyPropertyChange.AutoNotifyPropertyChanged"/>.
        /// </summary>
        OnChange,

        /// <summary>
        /// Don't save changes.
        /// </summary>
        ReadOnly
    }
}