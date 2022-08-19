  public class MultiData : Freezable
    {
        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new MultiData();
        }

        #endregion

        //=============
        public object Data1
        {
            get => GetValue(Data1Property);
            set => SetValue(Data1Property, value);
        }

        public static readonly DependencyProperty Data1Property =
            DependencyProperty.Register("Data1", typeof(object), typeof(MultiData), new UIPropertyMetadata(null));

        //=============
        public object Data2
        {
            get => GetValue(Data2Property);
            set => SetValue(Data2Property, value);
        }

        public static readonly DependencyProperty Data2Property =
            DependencyProperty.Register("Data2", typeof(object), typeof(MultiData), new UIPropertyMetadata(null));

        //=============
        public object Data3
        {
            get => GetValue(Data3Property);
            set => SetValue(Data3Property, value);
        }

        public static readonly DependencyProperty Data3Property =
            DependencyProperty.Register("Data3", typeof(object), typeof(MultiData), new UIPropertyMetadata(null));


        //=============
        public object Data4
        {
            get => GetValue(Data4Property);
            set => SetValue(Data4Property, value);
        }

        public static readonly DependencyProperty Data4Property =
            DependencyProperty.Register("Data4", typeof(object), typeof(MultiData), new UIPropertyMetadata(null));


        //=============
        public object Data5
        {
            get => GetValue(Data5Property);
            set => SetValue(Data5Property, value);
        }

        public static readonly DependencyProperty Data5Property =
            DependencyProperty.Register("Data5", typeof(object), typeof(MultiData), new UIPropertyMetadata(null));


        //=============
        public object Data6
        {
            get => GetValue(Data6Property);
            set => SetValue(Data6Property, value);
        }

        public static readonly DependencyProperty Data6Property =
            DependencyProperty.Register("Data6", typeof(object), typeof(MultiData), new UIPropertyMetadata(null));
    }