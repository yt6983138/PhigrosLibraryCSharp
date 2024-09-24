import com.github.unidbg.linux.android.dvm.*;

public class EmulatedJni extends AbstractJni {
    public DvmObject<?> callObjectMethodV(BaseVM vm, DvmObject<?> dvmObject, String signature, VaList vaList) {
        System.out.println(signature);
        if (signature.equals("java/lang/String->getAbsolutePath()Ljava/lang/String;")) {
            return new StringObject(vm, (String) dvmObject.getValue());
        }
        return super.callObjectMethodV(vm, dvmObject, signature, vaList);
    }

    public DvmObject<?> callObjectMethod(BaseVM vm, DvmObject<?> dvmObject, String signature, VarArg varArg) {
        System.out.println(signature);
        return super.callObjectMethod(vm, dvmObject, signature, varArg);
    }

    public DvmObject<?> FincClass(BaseVM vm, DvmObject<?> obj, String sign, VarArg arg) {
        System.out.println(sign);
        return null;
    }
}
