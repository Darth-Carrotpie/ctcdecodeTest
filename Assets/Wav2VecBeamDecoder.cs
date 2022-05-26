using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CTCBeamDecoder;
using CTCBeamDecoder.Models;
using UnityEngine;
public class Wav2VecBeamDecoder : MonoBehaviour {
    public static List<String> kLabels = new List<string>() {
        "<s>",
        "<pad>",
        "</s>",
        "<unk>",
        "|",
        "E",
        "T",
        "A",
        "O",
        "N",
        "I",
        "H",
        "S",
        "R",
        "D",
        "L",
        "U",
        "M",
        "W",
        "C",
        "F",
        "G",
        "Y",
        "P",
        "B",
        "V",
        "K",
        "'",
        "X",
        "J",
        "Q",
        "Z",
    };

    CTCBeamDecoder.Models.DecoderScorer scorer;
    CTCBeamDecoder.CTCDecoder decoder;
    public List<float[]> timestamps = new List<float[]>();

    private void Start() {
        FillRandomVals();
        scorer = new DecoderScorer(kLabels.ToArray());
        decoder = new CTCDecoder(blankId: 1, logProbsInput: true); //this depends if inputs are softmaxed or not. Check what it is!
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            int[] beam = GetTopBeam();
        }
    }

    public int[] GetTopBeam() {
        float[, ] timesteps = TimestepsToArray();
        float[, , ] singleBatchSteps = new float[1, timesteps.GetLength(0), timesteps.GetLength(1)];
        for (int i = 0; i < timesteps.GetLength(0); i++) {
            for (int j = 0; j < timesteps.GetLength(1); j++) {
                singleBatchSteps[0, i, j] = timesteps[i, j];
            }
        }
        CTCBeamDecoder.Models.DecoderResult result = decoder.Decode(singleBatchSteps, scorer);
        //print all beams to text here
        PrintResultBeams(result);
        //return top beam from the first item in the batch:
        int outLength = result.OutLens[0, 0];
        return result.BeamResults.SliceThirdDim(0, 0).Take(outLength).ToArray();
    }

    public void PrintResultBeams(CTCBeamDecoder.Models.DecoderResult result) {
        for (int i = 0; i < result.BeamResults.GetLength(1); i++) {
            int len = result.OutLens[0, i];
            int[] ids = result.BeamResults.SliceThirdDim(0, i).Take(len).ToArray();
            string text = String.Join("", ids.Select(x => labelIndexToOutput(x)));
            Debug.Log(text);
        }
    }

    public float[, ] TimestepsToArray() {
        int labelN = kLabels.Count;
        var n = timestamps.Count;
        float[, ] converted = new float[n, labelN];

        for (int i = 0; i < n; i++) {
            for (int j = 0; j < labelN; j++) {
                converted[i, j] = timestamps[i][j];
            }
        }
        return converted;
    }
    public static String labelIndexToOutput(int index) {
        if (index == 4) {
            return " ";
        } else if (index > 4 && index < kLabels.Count) {
            return kLabels[index];
        }
        return "";
    }
    void FillRandomVals() {
        int stepsCount = 50;
        for (int i = 0; i < stepsCount; i++) {
            float[] step = new float[kLabels.Count];
            for (int j = 0; j < kLabels.Count; j++) {
                step[j] = UnityEngine.Random.value;
            }
            timestamps.Add(step);
        }
    }
}