using System;
using System.Collections;
using System.Text;
using System.IO;

namespace MonoCov {

class LcovExporter {

	public static void Export(CoverageModel model, string path) {

	    using (TextWriter writer = new StreamWriter(path, false, Encoding.ASCII)) {
	    	foreach (ClassCoverageItem klass in model.Classes.Values) {
				if (klass.filtered || klass.hit + klass.missed == 0)
					continue;

				SourceFileCoverageData file = klass.sourceFile;
				writer.WriteLine("TN:{0}", "");
				// TODO relative path
				writer.WriteLine("SF:{0}", file.sourceFile);

				// FN records
				int fnCount = 0;
				int fnHitted = 0;
				foreach (MethodCoverageItem fn in file.methods) {
					if (fn.Class.filtered || fn.Class.hit + fn.Class.missed == 0)
						continue;

					fnCount++;
					if (fn.hit > 0) fnHitted++;

					string name = fn.FullName;
					writer.WriteLine("FN:{0},{1}", fn.startLine, name);
					writer.WriteLine("FNDA:{0},{1}", fn.hit, name);
				}

				writer.WriteLine("FNF:{0}", fnCount);
				writer.WriteLine("FNH:{0}", fnHitted);

				// lines coverage
				int[] coverage = file.Coverage;
				int ln = 0;
				int lh = 0;
				for (int i = 0; i < coverage.Length; i++) {
					int hits = coverage[i];
					if (hits < 0) continue; // comment

					writer.WriteLine("DA:{0},{1}", i + 1, hits);

					ln++;
					if (hits > 0) lh++;
				}

				writer.WriteLine("LF:{0}", ln);
				writer.WriteLine("LH:{0}", lh);
				writer.WriteLine("end_of_record");
			}
	    }
	}
}

}