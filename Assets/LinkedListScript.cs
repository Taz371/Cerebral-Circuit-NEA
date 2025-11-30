using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LinkedListScript<T>
{
    private class Node
    {
        public T value;
        public Node next;
        public Node prev;

        public Node(T Value)
        {
            value = Value;
            next = null;
            prev = null;
        }
    }

    private Node head;
    private Node tail;
    public int count;

    public LinkedListScript()
    {
        head = null;
        tail = null;
        count = 0;
    }

    public void AddFirst(T value)
    {
        Node node = new Node(value);

        if (head == null)
        {
            head = tail = node;
        }
        else
        {
            node.next = head;
            head.prev = node;
            head = node;
        }

        count++;
    }

    public void AddLast(T value)
    {
        Node node = new Node(value);

        if (tail == null)
        {
            head = tail = node;
        }
        else
        {
            node.prev = tail;
            tail.next = node;
            tail = node;
        }

        count++;
    }

    public T RemoveFirst()
    {
        if(head == null)
        {
            throw new InvalidOperationException("The list is empty.");
        }

        var value = head.value;
        head = head.next;

        if (head == null)
        {
            tail = null;
        }
        else
        {
            head.prev = null;
        }

        count--;
        return value;
    }

    public T RemoveLast()
    {
        if(tail == null)
        {
            throw new InvalidOperationException("The list is empty.");
        }

        var value = tail.value;
        tail = tail.prev;

        if (tail == null)
        {
            head = null;
        }
        else
        {
            tail.next = null;
        }

        count-- ;
        return value;
    }

    private void RemoveNode(Node node)
    {
        if (node.prev == null)
        {
            RemoveFirst();
        }
        else if (node.next == null)
        {
            RemoveLast();
        }
        else
        {
            node.prev.next = node.next;
            node.next.prev = node.prev;
            count--;
        }
    }

    public bool Contains(T data)
    {
        Node valueToBeCompared = head;

        while (valueToBeCompared != null)
        {
            if (EqualityComparer<T>.Default.Equals(valueToBeCompared.value, data))
            {
                return true;
            }
            valueToBeCompared = valueToBeCompared.next;
        }
        
        return false;
    }

    public void RemoveValue(T data)
    {
        Node valueToBeCompared = head;
        Node target = null;

        bool found = false;

        while (!found && valueToBeCompared!= null)
        {
            if (EqualityComparer<T>.Default.Equals(valueToBeCompared.value, data))
            {
                found = true;
                target = valueToBeCompared;
            }

            valueToBeCompared = valueToBeCompared.next;
        }

        if (found)
        {
            RemoveNode(target);
        }
    }

    public void Clear()
    {
        Node current = head;

        while (current != null)
        {
            Node next = current.next;

            current.next = null;
            current.prev = null;

            current = next;
        }

        head = null;
        tail = null;
        count = 0;
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }

            Node current = head;
            for (int i = 0; i < index; i++)
            {
                current = current.next;
            }

            return current.value;
        }
        set
        {
            if (index < 0 || index >= count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }

            Node current = head;
            for (int i = 0; i < index; i++)
            {
                current = current.next;
            }

            current.value = value;
        }
    }

    public T GetRandom()
    {
        if (count == 0)
        {
            throw new InvalidOperationException("Cannot get a random item from an empty list.");
        }

        int randomIndex = UnityEngine.Random.Range(0, count);
        return this[randomIndex];
    }
}