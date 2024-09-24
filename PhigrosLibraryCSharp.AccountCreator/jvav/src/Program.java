import com.github.unidbg.AbstractEmulator;
import com.github.unidbg.AndroidEmulator;
import com.github.unidbg.Module;
import com.github.unidbg.arm.backend.Unicorn2Factory;
import com.github.unidbg.linux.ARM32SyscallHandler;
import com.github.unidbg.linux.android.AndroidEmulatorBuilder;
import com.github.unidbg.linux.android.dvm.*;
import com.github.unidbg.linux.android.dvm.array.ByteArray;
import com.github.unidbg.unix.UnixSyscallHandler;
import org.apache.log4j.Level;
import org.apache.log4j.Logger;

import java.io.File;
import java.nio.charset.StandardCharsets;

public class Program {
    public static final boolean X86Mode = false;

    public static final String ApkPath = "C:\\Users\\tgg\\Desktop\\project\\taptap\\apk.apk";
    public static final String ElfPath = "tap-patch";
    public static final int ProcessStringOffset = X86Mode ? 0x21E0 : 0x2504;
    public static final int HashOffset = X86Mode ? 0x24E0 : 0x2598;
    public static final String Key1 = "PeCkE6Fu0B10Vm9BKfPfANwCUAn5POcs";
    /**
     * if ( !strcmp("com.taptap.pad", a2) )
     */
    public static final String Key2 = "pES9UHmwGsl5E6aBlTqYtjhXXzerZRL2";

    private static final byte[] header1234567 = new byte[]
            {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x23, 0x45, 0x67,
                    (byte) 0x89, (byte) 0xAB, (byte) 0xCD, (byte) 0xEF, (byte) 0xFE,
                    (byte) 0xDC, (byte) 0xBA, (byte) 0x98, 0x76, 0x54, 0x32, 0x10};

    private final AndroidEmulator emulator;

    private final VM vm;
    private final DalvikModule tapPatchSo;
    private final Module tapPatchModule;

    static {
        Logger.getLogger(ARM32SyscallHandler.class).setLevel(Level.DEBUG);
        Logger.getLogger(UnixSyscallHandler.class).setLevel(Level.DEBUG);
        Logger.getLogger(AbstractEmulator.class).setLevel(Level.DEBUG);
        Logger.getLogger(DalvikVM.class).setLevel(Level.DEBUG);
        Logger.getLogger(DalvikModule.class).setLevel(Level.DEBUG);
        Logger.getLogger(BaseVM.class).setLevel(Level.DEBUG);
        Logger.getLogger(DvmClass.class).setLevel(Level.DEBUG);
    }

    public Program() {
        emulator = AndroidEmulatorBuilder
                .for32Bit()
                .addBackendFactory(new Unicorn2Factory(true))
                .build();
        vm = emulator.createDalvikVM(new File(ApkPath));
        vm.setVerbose(true);
        vm.setJni(new EmulatedJni());
        tapPatchSo = vm.loadLibrary(ElfPath, true);
        tapPatchModule = tapPatchSo.getModule();
        //emulator.getBackend().addBreakPoint(0x1d60, null, false);
        //tapPatchSo.callJNI_OnLoad(emulator);
        System.out.println("jni loaded");
    }

    private static void PrintHexArray(byte[] data) {
        for (byte datum : data) {
            System.out.printf("%02X ", datum);
        }
        System.out.println();
    }

    public void FillHeader(byte[] buffer) {
        System.arraycopy(header1234567, 0, buffer, 0, header1234567.length);
    }

    public void ProcessString(byte[] buffer, byte[] data, int length) {
        tapPatchModule.callFunction(
                emulator,
                ProcessStringOffset,
                buffer,
                data,
                length);
    }

    public void Hash(byte[] in, byte[] out) {
        tapPatchModule.callFunction(
                emulator,
                HashOffset,
                new ByteArray(vm, in),
                new ByteArray(vm, out));
    }

    public void Main() {
        String testData = "X-UA=V=1&PN=TapTap&VN_CODE=206012000&LOC=TW&LANG=en_US&CH=default&UID=cd3da46d-5147-4946-aaf2-20922592994a&action=callback&android_id=60e6dc18c7bdc2da&cpu=armeabi-v7a&model=SM-G998B&name=samsung&nonce=mtps7&pn=TapTap&push_id=bcbef0fda23b4187a83ef8015fb9e182&screen=1024x768&supplier=1&time=1725791668&uuid=cd3da46d-5147-4946-aaf2-20922592994a&version=9";
        var buffer = new byte[104];
        FillHeader(buffer);
        System.out.println("filled header");
        ProcessString(buffer, testData.getBytes(StandardCharsets.UTF_8), 0x160);
        PrintHexArray(buffer);
        ProcessString(buffer, Key1.getBytes(StandardCharsets.UTF_8), 0x20);
        PrintHexArray(buffer);
    }

    public static void main(String[] args) {
        new Program().Main();
    }
}