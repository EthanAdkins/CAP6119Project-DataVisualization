import csv
import json

with open("docs/Data.csv", "r", encoding="utf-8") as observations:
    with open("docs/Data.json", "w", encoding="utf-8") as output:
        reader = csv.DictReader(observations)
        data = {
            "totalCount": 0,
            "numKingdoms": 0,
            "Kingdoms": []
        }
        
        for row in reader:
            modelLevel = row["Model Level"]
            kingdom = row["Kingdom"]
            phylum = row["Phylum"]
            dataClass = row["Class"]
            order = row["Order"]
            family = row["Family"]
            genus = row["Genus"]
            species = row["Species"]
            commonName = row["Common Name"]
            modelLocation = row["Model"]
            modelLevel = row["Model Level"]
            sedentary = bool(row["Sedentary"].strip().lower() == "TRUE")
            maxDepth = round(float(row["Max Depth (m)"]))
            minDepth = round(float(row["Min Depth (m)"]))
            count = int(row["count"])

            data["totalCount"] += count
            if not any(k["name"] == kingdom for k in data["Kingdoms"]):
                data["numKingdoms"] += 1
                data["Kingdoms"].append(
                    {
                        "name": kingdom,
                        "model": modelLocation if modelLevel == "Kingdom" else "",
                        "count": count,
                        "numPhylums": 1,
                        "Phylums": 
                        [
                            {
                                "name": phylum,
                                "model": modelLocation if modelLevel == "Phylum" else "",
                                "count": count,
                                "numClasses": 1,
                                "Classes": 
                                [
                                    {
                                        "name": dataClass,
                                        "model": modelLocation if modelLevel == "Class" else "",
                                        "count": count,
                                        "numOrders": 1,
                                        "Orders": 
                                        [
                                            {
                                                "name": order,
                                                "model": modelLocation if modelLevel == "Order" else "",
                                                "count": count,
                                                "numFamilies": 1,
                                                "Families": 
                                                [
                                                    {
                                                        "name": family,
                                                        "model": modelLocation if modelLevel == "Family" else "",
                                                        "count": count,
                                                        "numGenera": 1,
                                                        "Genera": 
                                                        [
                                                            {
                                                                "name": genus,
                                                                "model": modelLocation if modelLevel == "Genus" else "",
                                                                "count": count,
                                                                "numSpecies": 1,
                                                                "Species": 
                                                                [
                                                                    {
                                                                        "name": species,
                                                                        "commonName": commonName,
                                                                        "model": modelLocation if modelLevel == "Species" else "",
                                                                        "count": count,
                                                                        "sedentary": sedentary,
                                                                        "maxDepth": maxDepth,
                                                                        "minDepth": minDepth
                                                                    }
                                                                ]
                                                            }
                                                        ]
                                                    }
                                                ]
                                            }
                                        ]
                                    } 
                                ]
                            }
                        ]
                    }
                )
            else:
                k = next(k for k in data["Kingdoms"] if k["name"] == kingdom)
                k["count"] += count
                k["numPhylums"] += 1
                if not any(p["name"] == phylum for p in k["Phylums"]):
                    k["Phylums"].append(
                        {
                            "name": phylum,
                            "model": modelLocation if modelLevel == "Phylum" else "",
                            "count": count,
                            "numClasses": 1,
                            "Classes": 
                            [
                                {
                                    "name": dataClass,
                                    "model": modelLocation if modelLevel == "Class" else "",
                                    "count": count,
                                    "numOrders": 1,
                                    "Orders": 
                                    [
                                        {
                                            "name": order,
                                            "model": modelLocation if modelLevel == "Order" else "",
                                            "count": count,
                                            "numFamilies": 1,
                                            "Families": 
                                            [
                                                {
                                                    "name": family,
                                                    "model": modelLocation if modelLevel == "Family" else "",
                                                    "count": count,
                                                    "numGenera": 1,
                                                    "Genera": 
                                                    [
                                                        {
                                                            "name": genus,
                                                            "model": modelLocation if modelLevel == "Genus" else "",
                                                            "count": count,
                                                            "numSpecies": 1,
                                                            "Species": 
                                                            [
                                                                {
                                                                    "name": species,
                                                                    "commonName": commonName,
                                                                    "model": modelLocation if modelLevel == "Species" else "",
                                                                    "count": count,
                                                                    "sedentary": sedentary,
                                                                    "maxDepth": maxDepth,
                                                                    "minDepth": minDepth
                                                                }
                                                            ]
                                                        }
                                                    ]
                                                }
                                            ]
                                        }
                                    ]
                                } 
                            ]
                        }
                    )
                else:
                    p = next(p for p in k["Phylums"] if p["name"] == phylum)
                    p["count"] += count
                    p["numClasses"] += 1
                    if not any(c["name"] == dataClass for c in p["Classes"]):
                        p["Classes"].append(
                            {
                                "name": dataClass,
                                "model": modelLocation if modelLevel == "Class" else "",
                                "count": count,
                                "numOrders": 1,
                                "Orders": 
                                [
                                    {
                                        "name": order,
                                        "model": modelLocation if modelLevel == "Order" else "",
                                        "count": count,
                                        "numFamilies": 1,
                                        "Families": 
                                        [
                                            {
                                                "name": family,
                                                "model": modelLocation if modelLevel == "Family" else "",
                                                "count": count,
                                                "numGenera": 1,
                                                "Genera": 
                                                [
                                                    {
                                                        "name": genus,
                                                        "model": modelLocation if modelLevel == "Genus" else "",
                                                        "count": count,
                                                        "numSpecies": 1,
                                                        "Species": 
                                                        [
                                                            {
                                                                "name": species,
                                                                "commonName": commonName,
                                                                "model": modelLocation if modelLevel == "Species" else "",
                                                                "count": count,
                                                                "sedentary": sedentary,
                                                                "maxDepth": maxDepth,
                                                                "minDepth": minDepth
                                                            }
                                                        ]
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                ]
                            }
                        )
                    else:
                        c = next(c for c in p["Classes"] if c["name"] == dataClass)
                        c["count"] += count
                        c["numOrders"] += 1
                        if not any(o["name"] == order for o in c["Orders"]):
                            c["Orders"].append(
                                {
                                    "name": order,
                                    "model": modelLocation if modelLevel == "Order" else "",
                                    "count": count,
                                    "numFamilies": 1,
                                    "Families": 
                                    [
                                        {
                                            "name": family,
                                            "model": modelLocation if modelLevel == "Family" else "",
                                            "count": count,
                                            "numGenera": 1,
                                            "Genera": 
                                            [
                                                {
                                                    "name": genus,
                                                    "model": modelLocation if modelLevel == "Genus" else "",
                                                    "count": count,
                                                    "numSpecies": 1,
                                                    "Species": 
                                                    [
                                                        {
                                                            "name": species,
                                                            "commonName": commonName,
                                                            "model": modelLocation if modelLevel == "Species" else "",
                                                            "count": count,
                                                            "sedentary": sedentary,
                                                            "maxDepth": maxDepth,
                                                            "minDepth": minDepth
                                                        }
                                                    ]
                                                }
                                            ]
                                        }
                                    ]
                                }
                            )
                        else:
                            o = next(o for o in c["Orders"] if o["name"] == order)
                            o["count"] += count
                            o["numFamilies"] += 1
                            if not any(f["name"] == family for f in o["Families"]):
                                o["Families"].append(
                                    {
                                        "name": family,
                                        "model": modelLocation if modelLevel == "Family" else "",
                                        "count": count,
                                        "numGenera": 1,
                                        "Genera": 
                                        [
                                            {
                                                "name": genus,
                                                "model": modelLocation if modelLevel == "Genus" else "",
                                                "count": count,
                                                "numSpecies": 1,
                                                "Species": 
                                                [
                                                    {
                                                        "name": species,
                                                        "commonName": commonName,
                                                        "model": modelLocation if modelLevel == "Species" else "",
                                                        "count": count,
                                                        "sedentary": sedentary,
                                                        "maxDepth": maxDepth,
                                                        "minDepth": minDepth
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                )
                            else:
                                f = next(f for f in o["Families"] if f["name"] == family)
                                f["count"] += count
                                f["numGenera"] += 1
                                if not any(g["name"] == genus for g in f["Genera"]):
                                    f["Genera"].append(
                                        {
                                            "name": genus,
                                            "model": modelLocation if modelLevel == "Genus" else "",
                                            "count": count,
                                            "numSpecies": 1,
                                            "Species": 
                                            [
                                                {
                                                    "name": species,
                                                    "commonName": commonName,
                                                    "model": modelLocation if modelLevel == "Species" else "",
                                                    "count": count,
                                                    "sedentary": sedentary,
                                                    "maxDepth": maxDepth,
                                                    "minDepth": minDepth
                                                }
                                            ]
                                        }
                                    )
                                else:
                                    g = next(g for g in f["Genera"] if g["name"] == genus)
                                    g["count"] += count
                                    g["numSpecies"] += 1
                                    if not any(s["name"] == species for s in g["Species"]):
                                        g["Species"].append(
                                            {
                                                "name": species,
                                                "commonName": commonName,
                                                "model": modelLocation if modelLevel == "Species" else "",
                                                "count": count,
                                                "sedentary": sedentary,
                                                "maxDepth": maxDepth,
                                                "minDepth": minDepth
                                            }
                                        )
          
        print(data)
        
        json.dump(data, output, indent=4)  # Pretty formatting with 4 spaces
