using System.Linq;
using GamePackages.Core;
using NUnit.Framework;

namespace GamePackages.Tests
{
    public class QueueWithIndexerTest
    {
        [Test]
        public void QueueWithIndexer()
        {
            QueueWithIndexer<int> queue = new QueueWithIndexer<int>();
            Assert.AreEqual(0, queue.Count);
            Assert.Catch(() =>
            {
                int v = queue.Enqueue();
            });
            Assert.Catch(() =>
            {
                int v = queue[0];
            });


            queue.Queue(0);
            Assert.AreEqual(1, queue.Count);
            Assert.AreEqual(0, queue[0]);
            Assert.AreEqual(0, queue.ToArray()[0]);

            queue.Queue(1);
            queue.Queue(2);
            Assert.AreEqual(3, queue.Count);
            queue.Queue(3);
            Assert.AreEqual(4, queue.Capacity);
            Assert.AreEqual(4, queue.Count);

            int value = queue.Enqueue();
            Assert.AreEqual(0, value);
            Assert.AreEqual(3, queue.Count);
            queue.Queue(4);
            Assert.AreEqual(4, queue.Count);
            Assert.AreEqual(4, queue.Capacity);
            queue.Queue(5);
            Assert.AreEqual(5, queue.Count);
            Assert.AreEqual(8, queue.Capacity);

            for (int i = 0; i < 5; i++)
                Assert.AreEqual(i + 1, queue[i]);

            for (int i = 0; i < 5; i++)
            {
                int value1 = queue.Enqueue();
                Assert.AreEqual(i + 1, value1);
            }

            Assert.AreEqual(0, queue.Count);

            Assert.Catch(() =>
            {
                int v = queue.Enqueue();
            });
            Assert.Catch(() =>
            {
                int v = queue[0];
            });


            for (int i = 0; i < 55; i++)
                queue.Queue(i);

            Assert.AreEqual(55, queue.Count);
            Assert.AreEqual(54, queue[54]);
            Assert.AreEqual(0, queue[0]);


            for (int i = 0; i < 20; i++)
                queue.Enqueue();

            for (int i = 0; i < 20; i++)
                queue.Queue(i + 55);

            Assert.AreEqual(20, queue[0]);
            Assert.AreEqual(21, queue[1]);
            Assert.AreEqual(74, queue[54]);
            Assert.AreEqual(64, queue.Capacity);

            Assert.Catch(() => Enumerate(queue));
        }

        void Enumerate(QueueWithIndexer<int> queue)
        {
            int n = 0;
            foreach (int value in queue)
            {
                n++;
                if (n > 3)
                    queue.Queue(5);
            }
        }
    }
}
