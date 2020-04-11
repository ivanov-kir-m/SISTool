using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace ForelImplimentation
{
   class MainClass
   {
      // Размер выборки
      const int SIZE = 10;
      // Радиус поиска
      const double R_MIN = 0;
      const double R_MAX = (MAX - MIN) / 2.0;
      const double R_STEP = 1;
      // Пределы для генерации массива случайных чисел
      const double MIN = -10;
      const double MAX = +10;
      public static void Main (string[] args)
      {
         try {
            new MainClass ().Run ();
         } catch (Exception ex) {
            Console.WriteLine ("Необработанное исключение:\n{0}", ex.Message);
         }
      }
      /// <summary>
      /// Запускает цикл тестирования
      /// </summary>
      void Run ()
      {
         double[] data = genArray ();
         
         print ("Коллекция данных для тестирования:", data);
         
         Dictionary<double, int> clusterCount = new Dictionary<double, int> ();
         
         for (double r = R_MIN; r <= R_MAX; r += R_STEP) {
            clusterCount.Add (r, testFOREL (data, r));
         }
         
         double maxClusters = clusterCount.Max (a => { return a.Value; });
         
         Console.WriteLine ("График зависимости к-ва кластеров от R");
         foreach (var item in clusterCount) {
            Console.Write ("{0,5:0.00}:", item.Key);
            string str = new string ('*', (int)(item.Value / maxClusters * (80 - 11)));
            Console.WriteLine ("{0}{1,-5}", str, item.Value);
         }
      }
      /// <summary>
      /// Тестирует алгоритм FOREL
      /// </summary>
      /// <param name="array">
      /// Коллекция данных
      /// </param>
      /// <param name="r">
      /// Радиус поиска
      /// </param>
      /// <returns>
      /// Количество кластеров
      /// </returns>
      int testFOREL (double[] array, double r)
      {
         double[][] clusters = Forel.Solve (array, r, (o1, o2) => { return o1 - o2; }, a =>
         {
            double s = 0.0;
            foreach (double v in a)
               s += v;
            return s / a.Length;
         });
         print (string.Format ("Для параметра R={0} получены кластеры:", r), clusters);
         Console.WriteLine ("Всего {0} кластеров", clusters.Length);
         return clusters.Length;
      }
      // Генератор случайных чисел
      static Random random = new Random ();
      /// <summary>
      ///  Генерирует массив случайных чисел
      /// </summary>
      /// <returns>
      /// Сгенерированный массив
      /// </returns>
      double[] genArray ()
      {
         double[] result = new double[SIZE];
         
         for (int i = 0; i < SIZE; ++i) {
            result[i] = random.NextDouble () * (MAX - MIN) + MIN;
         }
         return result;
      }
      /// <summary>
      /// Выводит массив на консоль
      /// </summary>
      /// <param name="collection">
      /// Коллекция данных с интерфейсом IEnumerable
      /// </param>
      void print<T> (T collection) where T : IEnumerable
      {
         Console.Write ("(");
         foreach (var item in collection) {
            IEnumerable tmp = item as IEnumerable;
            if (tmp != null) {
               print (tmp);
            } else {
               Console.Write ("{0} ", item);
            }
         }
         Console.Write (")");
      }
      /// <summary>
      /// Выводит массив на консоль
      /// </summary>
      /// <param name="message">
      /// Сообщение перед выводом массива
      /// </param>
      /// <param name="collection">
      /// Коллекция данных с интерфейсом IEnumerable
      /// </param>
      void print<T> (string message, T collection) where T : IEnumerable
      {
         Console.WriteLine ("{0}", message);
         print (collection);
         Console.WriteLine ();
      }
      
   }
   /// <summary>
   /// Делегат для функции, определяющей расстояние между объектами
   /// </summary>
   public delegate double DistanceFunction<T> (T o1, T o2);
   /// <summary>
   /// Делегат для функции, определяющей центр масс объектов
   /// </summary>
   public delegate T CenterOfMassFunction<T> (T[] arr);
   /// <summary>
   /// Реализует алгоритм FOREL
   /// </summary>
   public static class Forel
   {
      private static Random random = new Random ();
      /// <summary>
      /// Решает задачу кластеризации
      /// </summary>
      /// <param name="array">
      /// Массив данных
      /// </param>
      /// <param name="r">
      /// Радиус
      /// </param>
      /// <param name="distance">
      /// Ф-я расчета дистанции между объектами
      /// </param>
      /// <param name="centerOfMass">
      /// Ф-я расчета центра масс
      /// </param>
      /// <returns>
      /// Массив кластеров
      /// </returns>
      public static T[][] Solve<T> (T[] array, double r, DistanceFunction<T> distance, CenterOfMassFunction<T> centerOfMass) where T : IEquatable<T>
      {
         r = Math.Abs (r);
         // Результат расчета
         List<T[]> result = new List<T[]> ();
         // Копируем данные в динамический список
         List<T> data = array.ToList ();
         // Пока есть не кластеризованные элементы
         while (data.Count > 0) {
            // Значение элемента выбранного в качестве центра
            T center;
            T centerNew = data[random.Next (data.Count)];
            // Получившийся кластер
            T[] cluster = null;
            do {
               center = centerNew;
               // Кластер собирается здесь               
               cluster = (from item in data
                  where distance (item, center) <= r
                  select item).ToArray ();
               // Считаем центр тяжести
               centerNew = centerOfMass (cluster);
            } while (!centerNew.Equals (center));
            result.Add (cluster);
            // Удаление кластеризованных элементов из выборки
            Array.ForEach (cluster, item => { data.Remove (item); });
         }
         return result.ToArray ();
      }
      
   }
}