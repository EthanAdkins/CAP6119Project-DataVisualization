using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class FloatingMenuController : MonoBehaviour
{
    public Transform positionTarget; // Assign to player's controller
    public GameObject menuUI; // Assign to floating menu
    public TextMeshProUGUI representativeNameText;
    public TextMeshProUGUI taxonomyText;
    public TextMeshProUGUI statsText;
    // public Image speciesImage;
    public InputActionReference toggleKey; // Assign to open/close key on controller

    void OnEnable()
    {
        toggleKey.action.performed += ToggleMenu;
        toggleKey.action.Enable();
    }

    void OnDisable()
    {
        toggleKey.action.performed -= ToggleMenu;
        toggleKey.action.Disable();
    }

    private bool isVisible = false;

    void Start()
    {
        menuUI.SetActive(false); // Start hidden
    }

    void Update()
    {
        // If visible, follow the player’s view
        if (isVisible)
        {
            FollowWrist();
        }
    }

    private void ToggleMenu(InputAction.CallbackContext context)
    {
        isVisible = !isVisible;
        menuUI.SetActive(isVisible);
    }

    public void ShowMenu()
    {
        isVisible = true;
        menuUI.SetActive(isVisible);
    }

    public void HideMenu()
    {
        isVisible = false;
        menuUI.SetActive(isVisible);
    }

    void FollowWrist()
    {
        if (positionTarget == null) return;

        // Offset from the wrist
        Vector3 offset = positionTarget.forward * 0.01f + positionTarget.up * .2f + positionTarget.right * .2f;
        transform.position = positionTarget.position + offset;

        // Face the camera
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    // Recursively builds ASCII phylogenetic tree
    private string BuildPhyloTree(object root, int depth = 0)
    {
        // Creates 4-space indentation for each depth level
        string Indent(int depth) => new string(' ', depth * 4);

        // Recursive version called after root is identified
        // - Skip to bottom of this definition for start of function logic
        string BuildPhyloTreeRecursive(object node, int depth)
        {
            if (node is Species s) // Base case
            {
                return $"{Indent(depth)}└── Species: {s.name}\n{Indent(depth + 2)} Common Name: {s.commonName}\n{Indent(depth + 2)} Depth range (meters): {s.minDepth}-{s.maxDepth}\n{Indent(depth + 2)} Count: {s.count}\n";
            }
            else if (node is Genus g)
            {
                string result = $"{Indent(depth)}└── Genus: {g.name}\n";
                foreach (var sp in g.Species)
                    result += BuildPhyloTreeRecursive(sp, depth + 1);
                return result;
            }
            else if (node is Family f)
            {
                string result = $"{Indent(depth)}└── Family: {f.name}\n";
                foreach (var ge in f.Genera)
                    result += BuildPhyloTreeRecursive(ge, depth + 1);
                return result;
            }
            else if (node is Order o)
            {
                string result = $"{Indent(depth)}└── Order: {o.name}\n";
                foreach (var fa in o.Families)
                    result += BuildPhyloTreeRecursive(fa, depth + 1);
                return result;
            }
            else if (node is TaxonClass c)
            {
                string result = $"{Indent(depth)}└── Class: {c.name}\n";
                foreach (var or in c.Orders)
                    result += BuildPhyloTreeRecursive(or, depth + 1);
                return result;
            }
            else if (node is Phylum p)
            {
                string result = $"{Indent(depth)}└── Phylum: {p.name}\n";
                foreach (var cl in p.Classes)
                    result += BuildPhyloTreeRecursive(cl, depth + 1);
                return result;
            }
            else if (node is Kingdom k)
            {
                string result = $"Kingdom: {k.name}\n";
                foreach (var ph in k.Phyla)
                    result += BuildPhyloTreeRecursive(ph, 1);
                return result;
            }
            else return "";
        }

        if (root is Species s) // Base case
        {
            return $"Species: {s.name}\nCommon Name: {s.commonName}\nDepth range (meters): {s.minDepth}-{s.maxDepth}\nCount: {s.count}\n";
        }
        else if (root is Genus g)
        {
            string result = $"Genus: {g.name}\n";
            foreach (var sp in g.Species)
                result += BuildPhyloTreeRecursive(sp, depth + 1);
            return result;
        }
        else if (root is Family f)
        {
            string result = $"Family: {f.name}\n";
            foreach (var ge in f.Genera)
                result += BuildPhyloTreeRecursive(ge, depth + 1);
            return result;
        }
        else if (root is Order o)
        {
            string result = $"Order: {o.name}\n";
            foreach (var fa in o.Families)
                result += BuildPhyloTreeRecursive(fa, depth + 1);
            return result;
        }
        else if (root is TaxonClass c)
        {
            string result = $"Class: {c.name}\n";
            foreach (var or in c.Orders)
                result += BuildPhyloTreeRecursive(or, depth + 1);
            return result;
        }
        else if (root is Phylum p)
        {
            string result = $"Phylum: {p.name}\n";
            foreach (var cl in p.Classes)
                result += BuildPhyloTreeRecursive(cl, depth + 1);
            return result;
        }
        else if (root is Kingdom k)
        {
            string result = $"Kingdom: {k.name}\n";
            foreach (var ph in k.Phyla)
                result += BuildPhyloTreeRecursive(ph, depth + 1);
            return result;
        }
        else return "No taxonomic info found.";
    }

    public void SetInfo(SpeciesManager manager)
    {
        if (manager == null || manager.root == null) return;
        
        representativeNameText.text = manager.SpeciesName;
        
        (int count, float minDepth, float maxDepth) = manager.root switch
        {
            Species s     => (s.count, s.minDepth, s.maxDepth),
            Genus g       => (g.count, g.minDepth, g.maxDepth),
            Family f      => (f.count, f.minDepth, f.maxDepth),
            Order o       => (o.count, o.minDepth, o.maxDepth),
            TaxonClass c  => (c.count, c.minDepth, c.maxDepth),
            Phylum p      => (p.count, p.minDepth, p.maxDepth),
            Kingdom k     => (k.count, k.minDepth, k.maxDepth),
            _             => (0, 0f, 0f)
        };
        statsText.text = $"Observation Count: {count}\nMinimum Depth Observed: {minDepth} meters\nMaximum Depth Observed: {maxDepth} meters";

        taxonomyText.text = BuildPhyloTree(manager.root);
    }
}
