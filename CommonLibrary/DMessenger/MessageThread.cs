using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DMessenger
{
    public sealed class MessageThread : IReadOnlyList<Message>
    {
        private readonly List<Message> messages = new List<Message>();

        public Message this[int index] => messages[index];

        public int Count => messages.Count;
        public Guid ThreadId { get; }

        /// <summary>
        /// 作成または受信したメッセージをスレッドに追加します。
        /// </summary>
        /// <param name="value"></param>
        public bool AddOrUpdate(Message value)
        {
            lock (messages)
            {
                // 同一IDのメッセージがないか確認する
                foreach (var item in messages)
                {
                    if (item.MessageId == value.MessageId)
                    {
                        if (item.UpdateTime < value.UpdateTime)
                        {
                            messages.Remove(item); // 旧メッセージを削除
                            break;
                        }
                        else
                            return false;
                    }
                }

                // 確認できたので、スレッドに追加
                for (int i = messages.Count - 1; i >= 0; i--)
                {
                    if (messages[i].UpdateTime < value.UpdateTime)
                    {
                        messages.Insert(i + 1, value);
                        return true;
                    }
                }
                messages.Add(value); // 一番新しいメッセージの場合
                return true;
            }
        }

        public IEnumerator<Message> GetEnumerator()
        {
            lock (messages)
            {
                return (IEnumerator<Message>)messages.ToArray().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
