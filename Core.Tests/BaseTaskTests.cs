using System;
using System.Threading.Tasks;
using Xunit;

namespace ZidiumServerMonitor.Tests
{
    public class BaseTaskTests
    {
        /// <summary>
        /// Проверка выполнения задачи через указанный интервал
        /// </summary>
        [Fact]
        public async Task ExecutionPeriodTest()
        {
            // Запустим тестовую задачу, которая выполняется каждые 10 секунд
            var task = new TestTask();
            task.Start();

            // Подождём 1 секунду
            await Task.Delay(TimeSpan.FromSeconds(1));

            // Проверим, что задача выполнилась сразу
            Assert.Equal(1, task.ExecutionCount);

            // Подождём 10 секунд
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Проверим, что задача выполнилась
            Assert.Equal(2, task.ExecutionCount);

            // Подождём ещё 10 секунд
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Проверим что задача выполнилась ещё раз
            Assert.Equal(3, task.ExecutionCount);

            // Остановим задачу
            task.Stop();

            // Подождём ещё 10 секунд
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Проверим, что задача больше не выполняется
            Assert.Equal(3, task.ExecutionCount);

        }
    }
}
