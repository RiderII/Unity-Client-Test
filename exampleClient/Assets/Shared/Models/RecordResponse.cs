using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordResponse {
    public List<MapReport> records;
    public string totals;

    public RecordResponse(){}
    public RecordResponse(List<MapReport> records, string totals) {
        this.records = records;
        this.totals = totals;
    }


}
