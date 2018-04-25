using System.Collections.Generic;
using System.Drawing;

namespace TermRules
{
    public class Range
    {
        public Range(Point inf)
        {
            Inf = inf;
        }
        public Point Inf { get; set; }
        public static bool operator <(Range l, Range r)
        {
            if (l == null || r == null) return false; //Нужно написать исключение!
            return (((r.Inf.X > l.Inf.X) && (r.Inf.Y <= l.Inf.Y)) || ((r.Inf.X >= l.Inf.X) && (r.Inf.Y < l.Inf.Y)));
        }
        public static bool operator >(Range l, Range r)
        {
            if (l == null || r == null) return false;
            return (((r.Inf.X < l.Inf.X) && (r.Inf.Y >= l.Inf.Y)) || ((r.Inf.X <= l.Inf.X) && (r.Inf.Y > l.Inf.Y)));
        }
        public static bool operator <=(Range l, Range r)
        {
            if (l == null || r == null) return false;
            return (((r.Inf.X > l.Inf.X) && (r.Inf.Y <= l.Inf.Y)) 
                || ((r.Inf.X >= l.Inf.X) && (r.Inf.Y < l.Inf.Y))) 
                || (l.Inf.X == r.Inf.X && l.Inf.Y == r.Inf.Y);
        }
        public static bool operator >=(Range l, Range r)
        {
            if (l == null || r == null) return false;
            return (((r.Inf.X < l.Inf.X) && (r.Inf.Y >= l.Inf.Y)) || ((r.Inf.X <= l.Inf.X) && (r.Inf.Y > l.Inf.Y))) || (l.Inf.X == r.Inf.X && l.Inf.Y == r.Inf.Y);
        }
        public static bool Equals(Range l, Range r)
        {
            if (l == null && r == null) return true;
            if (l == null || r == null) return false;
            return (l.Inf.X == r.Inf.X && l.Inf.Y == r.Inf.Y);
        }
        public static bool NotEquals(Range l, Range r)
        {
            if (l == null || r == null) return false;
            return (l.Inf.X != r.Inf.X || l.Inf.Y != r.Inf.Y);
        }
        public bool include_range(Range r)
        {
            if (r == null) return false;
            return (((Inf.X < r.Inf.X ) && ( r.Inf.Y<= Inf.Y)) || ((Inf.X <= r.Inf.X) && (r.Inf.Y < Inf.Y)));
            //return (((this.inf.X > r.inf.X) && (this.inf.Y <= r.inf.Y)) || ((this.inf.X >= r.inf.X) && (this.inf.Y < r.inf.Y)));
        }
    }
    public class TermTree
    {
        //___________________________________________________________________________________________________________________
        // Основа класса
        //___________________________________________________________________________________________________________________

        public Range Range { get; set; }
        public HashSet<int> IndexElement;
        public TermTree Left { get; set; }
        public TermTree Right { get; set; }
        public TermTree Parent { get; set; }
        public TermTree() {
            IndexElement = new HashSet<int>();
            Left = null;
            Right = null;
            Parent = null;
            Range = null;
        }
        ~TermTree() { }
        //___________________________________________________________________________________________________________________
        //___________________________________________________________________________________________________________________

        //___________________________________________________________________________________________________________________
        // Добавление интервала
        //___________________________________________________________________________________________________________________
        /// <summary>
        /// Добавляет заданный интервал в бинарное дерево поиска
        /// </summary>
        /// <param name="new_range">Добавляемый интервал</param>
        /// <returns></returns>
        public TermTree AddRange(Range newRange)
        {
            TermTree node = null;
            TermTree parent = null;
            if (Range.Equals(Range, null) || Range.Equals(Range,newRange))
            {
                Range = newRange;
                return this;
            }
            if (newRange <= Range)
            {
                //if (left == null) left = new TermTree();
                if (Left == null) Left = new TermTree();
                parent = this;
                node = Left;
                //return AddRange(new_range, left, this);
            }
            else
            {
                //if (right == null) right = new TermTree();
                if (Right == null) Right = new TermTree();                
                parent = this;
                node = Right;
                //return AddRange(new_range, right, this);
            }
            while(true)
            {
                //if (node.range == null || node.range == new_range)
                if (Range.Equals(node.Range, null) || Range.Equals(node.Range, newRange))
                {
                    node.Range = newRange;
                    node.Parent = parent;
                    return node;
                }
                if (newRange <= node.Range)
                {
                    if (node.Left == null) node.Left = new TermTree();
                    parent = node;
                    node = node.Left;
                    //return AddRange(new_range, node.left, node);
                }
                else
                {
                    if (node.Right == null) node.Right = new TermTree();
                    parent = node;
                    node = node.Right;
                    //return AddRange(new_range, node.right, node);
                }
            }
        }
        /// <summary>
        /// Вставляет интервал в определённый узел дерева
        /// </summary>
        /// <param name="new_range">Интервал</param>
        /// <param name="node">Целевой узел для вставки</param>
        /// <param name="parent">Родительский узел</param>
        //private int AddRange(Range new_range, TermTree node, TermTree parent)
        //{
        //    //if (node.range == null || node.range == new_range)
        //    if (Range.Equals(node.range, null) || Range.Equals(node.range, new_range))
        //    {
        //        node.range = new_range;
        //        node.parent = parent;
        //        return 0;
        //    }
        //    if (new_range <= node.range)
        //    {
        //        if (node.left == null) node.left = new TermTree();
        //        return AddRange(new_range, node.left, node);
        //    }
        //    else
        //    {
        //        if (node.right == null) node.right = new TermTree();
        //        return AddRange(new_range, node.right, node);
        //    }
        //}
        /// <summary>
        /// Уставляет узел в определённый узел дерева
        /// </summary>
        /// <param name="new_range">Вставляемый узел</param>
        /// <param name="node">Целевой узел</param>
        /// <param name="parent">Родительский узел</param>
        private TermTree AddRange(TermTree newRange, TermTree node, TermTree parent)
        {
            while (true)
            {
                //if (node.range == null || node.range == new_range.range)
                if (Range.Equals(node.Range, null) || Equals(node.Range, newRange))
                {
                    node.Range = newRange.Range;
                    node.Left = newRange.Left;
                    node.Right = newRange.Right;
                    node.Parent = parent;
                    return node;
                }
                if (newRange.Range <= node.Range)
                {
                    if (node.Left == null) node.Left = new TermTree();
                    parent = node;
                    node = node.Left;
                    //return AddRange(new_range, node.left, node);
                }
                else
                {
                    if (node.Right == null) node.Right = new TermTree();
                    parent = node;
                    node = node.Right;
                    //return AddRange(new_range, node.right, node);
                }
            }
        }
        //___________________________________________________________________________________________________________________
        //___________________________________________________________________________________________________________________

        //___________________________________________________________________________________________________________________
        // Удаление интервала
        //___________________________________________________________________________________________________________________
        public enum BinSide
        {
            Left,
            Right
        }
        /// <summary>
        /// Определяет, в какой ветви для родительского лежит данный узел
        /// </summary>
        /// <param name="delete_range"></param>
        /// <returns></returns>
        private BinSide? NodeForParent(TermTree deleteRange)
        {
            if (deleteRange.Parent == null) return null;
            if (deleteRange.Parent.Left == deleteRange) return BinSide.Left;
            if (deleteRange.Parent.Right == deleteRange) return BinSide.Right;
            return null;
        }
        /// <summary>
        /// Удаляет узел из дерева
        /// </summary>
        /// <param name="delete_range">Удаляемый узел</param>
        public void DeleteRange(TermTree deleteRange)
        {
            if (deleteRange == null) return;
            var me = NodeForParent(deleteRange);
            //Если у узла нет дочерних элементов, его можно смело удалять
            if (deleteRange.Left == null && deleteRange.Right == null)
            {
                if (me == BinSide.Left)
                {
                    deleteRange.Parent.Left = null;
                }
                else if (me == BinSide.Right)
                {
                    deleteRange.Parent.Right = null;
                }
                else if (me == null)
                {
                    deleteRange = null;
                }
                return;
            }
            //Если нет левого дочернего, то правый дочерний становится на место удаляемого
            if (deleteRange.Left == null)
            {
                if (me == BinSide.Left)
                {
                    deleteRange.Parent.Left = deleteRange.Right;
                }
                else if (me == BinSide.Right)
                {
                    deleteRange.Parent.Right = deleteRange.Right;
                }

                deleteRange.Right.Parent = deleteRange.Parent;
                return;
            }
            //Если нет правого дочернего, то левый дочерний становится на место удаляемого
            if (deleteRange.Right == null)
            {
                if (me == BinSide.Left)
                {
                    deleteRange.Parent.Left = deleteRange.Left;
                }
                else if (me == BinSide.Right)
                {
                    deleteRange.Parent.Right = deleteRange.Left;
                }

                deleteRange.Left.Parent = deleteRange.Parent;
                return;
            }

            //Если присутствуют оба дочерних узла
            //то правый ставим на место удаляемого
            //а левый вставляем в правый

            if (me == BinSide.Left)
            {
                deleteRange.Parent.Left = deleteRange.Right;
            }
            if (me == BinSide.Right)
            {
                deleteRange.Parent.Right = deleteRange.Right;
            }
            if (me == null)
            {
                var bufleft = deleteRange.Left;
                var bufrightleft = deleteRange.Right.Left;
                var bufrightright = deleteRange.Right.Right;
                deleteRange.Range = deleteRange.Right.Range;
                deleteRange.Right = bufrightright;
                deleteRange.Left = bufrightleft;
                AddRange(bufleft, deleteRange, deleteRange);
            }
            else
            {
                deleteRange.Right.Parent = deleteRange.Parent;
                AddRange(deleteRange.Left, deleteRange.Right, deleteRange.Right);
            }
        }
        /// <summary>
        /// Удаляет значение из дерева
        /// </summary>
        /// <param name="delete_range">Удаляемое значение</param>
        public void DeleteRange(Range deleteRange)
        {
            var removeNode = FindRange(deleteRange);
            if (removeNode != null)
            {
                DeleteRange(removeNode);
            }
        }
        //___________________________________________________________________________________________________________________
        //___________________________________________________________________________________________________________________

        //___________________________________________________________________________________________________________________
        // Поиск интервала
        //___________________________________________________________________________________________________________________
        /// <summary>
        /// Ищет узел с заданным значением
        /// </summary>
        /// <param name="find_range">Значение для поиска</param>
        /// <returns></returns>
        public TermTree FindRange(Range findRange)
        {
            if (this == null || Range == null) return null;
            TermTree node = null;
            if (Range.Equals(Range, findRange)) return this;
            if (findRange <= Range)
            {
                node = Left;
            }
            else
                node = Right;
            //if (range == find_range) return this;
            while (true)
            {
                if (node == null) return null;
                if (Range.Equals(node.Range, findRange)) return node;
                //if (node.range == find_range) return node;
                if (findRange <= node.Range)
                {
                    node = node.Left;
                }
                else
                    node = node.Right;
            }
        }
        /// <summary>
        /// Ищет значение в определённом узле
        /// </summary>
        /// <param name="find_range">Значение для поиска</param>
        /// <param name="node">Узел для поиска</param>
        /// <returns></returns>
        //public TermTree FindRange(Range find_range, TermTree node)
        //{
        //    if (node == null) return null;

        //    if (Range.Equals(node.range, find_range)) return node;
        //    //if (node.range == find_range) return node;
        //    if (find_range <= node.range)
        //    {
        //        return FindRange(find_range, node.left);
        //    }
        //    return FindRange(find_range, node.right);
        //}
        /// <summary>
        /// Ищет узел с заданным значением
        /// </summary>
        /// <param name="find_range">Значение для поиска</param>
        /// <returns></returns>
        public TermTree FindRangeExtension(Range findRange)
        {
            if (this == null || Range == null) return null;
            TermTree node = null;
            if (Range.include_range(findRange)) return this;
            if (findRange <= Range)
            {
                //return FindRangeExtension(find_range, left);
                node = Left;
            }
            else
                node = Right;
            while (true)
            {
                if (node == null) return null;
                if (node.Range.include_range(findRange)) return node;
                if (findRange <= node.Range)
                {
                    //return FindRangeExtension(find_range, node.left);
                    node = node.Left;
                }
                else
                    node = node.Right;
                //return FindRangeExtension(find_range, node.right);
            }
            //return FindRangeExtension(find_range, right);
        }
        /// <summary>
        /// Ищет значение в определённом узле
        /// </summary>
        /// <param name="find_range">Значение для поиска</param>
        /// <param name="node">Узел для поиска</param>
        /// <returns></returns>
        //public TermTree FindRangeExtension(Range find_range, TermTree node)
        //{
        //    if (node == null) return null;

        //    if (node.range.include_range(find_range)) return node;
        //    if (find_range <= node.range)
        //    {
        //        return FindRangeExtension(find_range, node.left);
        //    }
        //    return FindRangeExtension(find_range, node.right);
        //} 
        //___________________________________________________________________________________________________________________
        //___________________________________________________________________________________________________________________

        //___________________________________________________________________________________________________________________
        // Подсчет элементов в дереве
        //___________________________________________________________________________________________________________________
        /// <summary>
        /// Количество элементов в дереве
        /// </summary>
        /// <returns></returns>
        public long CountElements()
        {
            return CountElements(this);
        }
        /// <summary>
        /// Количество элементов в определённом узле
        /// </summary>
        /// <param name="node">Узел для подсчета</param>
        /// <returns></returns>
        private long CountElements(TermTree node)
        {
            long count = 1;
            if (node.Right != null)
            {
                count += CountElements(node.Right);
            }
            if (node.Left != null)
            {
                count += CountElements(node.Left);
            }
            return count;
        }

    }
    
}
