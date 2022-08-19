#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Prism.Events;

namespace Prism.Commands
{
    public static class CommandToPubSubEvent
    {
        public static IEventAggregator? EventAggregator { get; set; }


        public static void RegistEvent<TEvent>() where TEvent : EventBase
        {
            EventTypes.Add(typeof(TEvent).Name, typeof(TEvent));
        }

        private static Dictionary<string, Type> EventTypes { get; } = new();


        private static DelegateCommand<PrismEventParameters>? _pubSubEventCommand;

        public static DelegateCommand<PrismEventParameters> PubSubEventCommand =>
            _pubSubEventCommand ??= new DelegateCommand<PrismEventParameters>(PrismEventExecute);


        private static void PrismEventExecute(PrismEventParameters prismEventParameters)
        {
            if (EventAggregator is null)
            {
                throw new ApplicationException("没有指定 EventAggregator 的实例,无法转发事件");
            }

            var eventType = prismEventParameters.EventType;

            var payload = prismEventParameters.Payload;

            PublishEvent(eventType, payload);
        }

        private static void PublishEvent(Type eventType, object payload)
        {
            //如果是泛型事件
            if (eventType.HasImplementedRawGeneric(typeof(PubSubEvent<>)))
            {
                var getEventMethod = EventAggregator
                    .GetType()!
                    .GetMethod("GetEvent")!
                    .MakeGenericMethod(eventType);

                //拿到 event 对象的实例
                var subEventObject = getEventMethod.Invoke(EventAggregator, new object[] { });

                var publishEvent = eventType
                    .GetMethod("Publish");

                publishEvent!.Invoke(subEventObject, new[] {payload});
            }
            else if (eventType.HasImplementedRawGeneric(typeof(PubSubEvent)))
            {
                var getEventMethod = EventAggregator
                    .GetType()!
                    .GetMethod("GetEvent")!
                    .MakeGenericMethod(eventType);

                //拿到 event 对象的实例
                var subEventObject = getEventMethod!.Invoke(EventAggregator, new object[] { });

                var publishEvent = eventType.GetMethod("Publish");

                publishEvent!.Invoke(subEventObject, new object[] { });
            }
        }


        /// <summary>
        /// 判断指定的类型 <paramref name="type"/> 是否是指定泛型类型的子类型，或实现了指定泛型接口。
        /// </summary>
        /// <param name="type">需要测试的类型。</param>
        /// <param name="generic">泛型接口类型，传入 typeof(IXxx&lt;&gt;)</param>
        /// <returns>如果是泛型接口的子类型，则返回 true，否则返回 false。</returns>
        public static bool HasImplementedRawGeneric(this Type type, Type generic)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (generic == null) throw new ArgumentNullException(nameof(generic));

            // 测试接口。
            var isTheRawGenericType = type.GetInterfaces().Any(IsTheRawGenericType);
            if (isTheRawGenericType) return true;

            // 测试类型。
            while (type != null && type != typeof(object))
            {
                isTheRawGenericType = IsTheRawGenericType(type);
                if (isTheRawGenericType) return true;
                type = type.BaseType;
            }

            // 没有找到任何匹配的接口或类型。
            return false;

            // 测试某个类型是否是指定的原始接口。
            bool IsTheRawGenericType(Type test)
                => generic == (test.IsGenericType ? test.GetGenericTypeDefinition() : test);
        }
    }

    public class PrismEventParameters : DependencyObject
    {
        public Type EventType { get; set; }


        //public object Payload
        //{
        //    get => (object) GetValue(PayloadProperty);
        //    set => SetValue(PayloadProperty, value);
        //}

        //public static readonly DependencyProperty PayloadProperty =
        //    DependencyProperty.Register(
        //        "Payload",
        //        typeof(object),
        //        typeof(PrismEventParameters),
        //        new PropertyMetadata(default(object), PayloadPropertyChanged)
        //    );

        //private static void PayloadPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is PrismEventParameters thisPrismEventParameters &&
        //        e.OldValue is object oldValue &&
        //        e.NewValue is object newValue)
        //    {
        //    }
        //}


        public object Payload
        {
            get => GetValue(PayloadProperty);
            set => SetValue(PayloadProperty, value);
        }

        public static readonly DependencyProperty PayloadProperty =
            DependencyProperty.Register(
                "Payload",
                typeof(object),
                typeof(PrismEventParameters),
                new FrameworkPropertyMetadata(
                    default,
                    FrameworkPropertyMetadataOptions.Inherits,
                    PayloadPropertyChanged)
            );

        private static void PayloadPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PrismEventParameters thisPrismEventParameters &&
                e.OldValue is object oldValue &&
                e.NewValue is object newValue)
            {
            }
        }
    }
}


// 例子
// <Button
//         Command="{x:Static commands:CommandToPubSubEvent.PubSubEventCommand}">


//             <Button.Resources>
//                 <common:BindingProxy
//                     x:Key="BindingProxy"
//                     Data="{Binding}" />
//             </Button.Resources>

//             <Button.CommandParameter>
//                 <commands:PrismEventParameters
//                     EventType="{x:Type 事件类型}"
//                     Payload="{Binding Source={StaticResource BindingProxy},
//                                       Path=Data.(XXX)}" />
//             </Button.CommandParameter>
// </Button>