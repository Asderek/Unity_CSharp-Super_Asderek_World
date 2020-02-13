using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPath : RouteFloor
{

    [System.Serializable]
    public class Path
    {
        public GameObject myPath;
        public Switch source;
        public List<Path> options;

        [HideInInspector]
        public Path parent;
        [HideInInspector]
        public int destiny;
        [HideInInspector]
        public int future;
        [HideInInspector]
        public bool inUse;
        
        public List<Transform> Get()
        {
            List<Transform> steps = new List<Transform>();
            steps = new List<Transform>(myPath.GetComponentsInChildren<Transform>());
            steps.Remove(myPath.transform);
            return steps;
        }

        public Transform GetLast()
        {
            List<Transform> steps = Get();
            return steps[steps.Count - 1];
        }

        public Transform GetFirst()
        {
            List<Transform> steps = Get();
            return steps[0];
        }

        public override string ToString()
        {
            return "Path: " + myPath;
        }

        public void Next()
        {
            future++;
            if (future >= options.Count)
            {
                future = 0;
            }
        }

        public bool Update()
        {
            bool ret = destiny == future;
            destiny = future;
            return ret;
        }

        public void Start()
        {
            future = -1;
            foreach (Path p in options)
                p.Start();
        }

        public void SetParent(Path path)
        {
            parent = path;
            foreach (Path p in options)
                p.SetParent(this);
        }

        public void SetSwitch(Switch s)
        {
            if (s == source)
            {
                Next();
                if (future == 0)
                {
                    s.Reset();
                }
                
                if (!CheckBusy() || inUse)
                {
                    Update();
                }
                    
            }
            else
            {
                foreach (Path p in options)
                    p.SetSwitch(s);
            }
        }

        public bool CheckBusy()
        {
            if (inUse)
                return true;

            foreach (Path p in options)
            {
                if (p.CheckBusy())
                    return true;
            }
            return false;
        }
    }

    public Path path;
    private Path currentPath;
    private List<Transform> linePos = new List<Transform>();
    private List<Transform> redraw = new List<Transform>();

    protected override void Start()
    {
        path.Start();
        route = path.myPath;
        base.Start();

        path.SetParent(null);
        currentPath = path;
        currentPath.inUse = true;
    }

    protected override void HandlePoint(Transform t)
    {
        if (redraw.Contains(t))
        {
            RedoSteps();
            redraw.Remove(t);
        }
    }

    protected override void HandleEdge()
    {
        steps.Clear();

        if (direction)
        {

            if (currentPath.options.Count == 0)
            {
                direction = false;
            }
            else
            {
                steps.Add(currentPath.GetLast());

                currentPath.inUse = false;
                currentPath = currentPath.options[currentPath.destiny];
                currentPath.inUse = true;
            }
            steps.AddRange(currentPath.Get());

        }
        else
        {
            if (currentPath.parent ==  null)
            {
                direction = true;
                steps.AddRange(currentPath.Get());
            }
            else
            {
                if (currentPath.parent.options[currentPath.parent.future] == currentPath)
                {
                    currentPath.inUse = false;
                    currentPath = currentPath.parent;
                    currentPath.inUse = true;
                    steps.AddRange(currentPath.Get());
                    steps.Add(currentPath.options[currentPath.destiny].GetFirst());
                }
                else
                {
                    steps.Add(currentPath.GetFirst());
                    steps.Add(currentPath.parent.GetLast());

                    currentPath.parent.Update();

                    currentPath.inUse = false;
                    currentPath = currentPath.parent.options[currentPath.parent.destiny];
                    currentPath.inUse = true;
                    steps.AddRange(currentPath.Get());
                    direction = true;

                }


            }
        }


        if (direction)
        {
            currentStep = 0;
        }
        else
        {
            currentStep = steps.Count - 1;
        }

    }

    public virtual void ChangePath(Switch source)
    {
        path.SetSwitch(source);

        if (currentPath.options.Count != 0)
        {
            if (direction == false)
            {
                if (currentStep == steps.Count - 2)
                {
                    redraw.Add(steps[steps.Count - 2]);
                    return;
                }
            }
        }
    
        if (direction)
        {
            if (steps.Count >= currentPath.Get().Count + 2)
            {
                redraw.Add(steps[0]);
            }
        }

        RedoSteps();
    }

    protected virtual void RedoSteps()
    {
        Path aux = path;
        linePos.Clear();
        while (aux.options.Count != 0)
        {
            aux = aux.options[aux.destiny];
        }

        do
        {
            List<Transform> auxT = aux.Get();
            auxT.Reverse();
            linePos.AddRange(auxT);

            if (aux.parent == null)
            {
                break;
            }

            if (aux.parent.options[aux.parent.future] != aux)
            {

                aux = aux.parent.options[aux.parent.future];
                linePos.Add(aux.parent.GetLast());
                redraw.Add(aux.parent.GetLast());
                linePos.AddRange(aux.Get());
                while (aux.options.Count != 0)
                {
                    aux = path.options[path.destiny];
                    linePos.AddRange(aux.Get());
                }
                break;
            }

            aux = aux.parent;
        } while (true);

        //linePos.Reverse();
        DrawSteps(linePos);
    }
}
