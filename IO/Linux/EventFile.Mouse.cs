namespace BxNiom.Linux;

public static class EventFileMouseEx {
    public static void RelMouseMove(this EventFile file, int x, int y) {
        if (x != 0) {
            file.Write(LinuxEvents.EV_REL, LinuxEvents.REL_X, x);
        }

        if (y != 0) {
            file.Write(LinuxEvents.EV_REL, LinuxEvents.REL_Y, y);
        }

        if (x != 0 || y != 0) {
            file.Write(LinuxEvents.EV_SYN, 0, 0);
        }
    }

    public static void RelMouseWheel(this EventFile file, int delta, bool hiRes = true) {
        file.Write(LinuxEvents.EV_REL, hiRes ? LinuxEvents.REL_WHEEL_HI_RES : LinuxEvents.REL_WHEEL, delta);
        file.Write(LinuxEvents.EV_SYN, 0, 0);
    }

    public static void RelMouseHWheel(this EventFile file, int delta, bool hiRes = true) {
        file.Write(LinuxEvents.EV_REL, hiRes ? LinuxEvents.REL_HWHEEL_HI_RES : LinuxEvents.REL_HWHEEL, delta);
        file.Write(LinuxEvents.EV_SYN, 0, 0);
    }

    public static void MouseButtonDown(this EventFile file, int button) {
        file.Write(LinuxEvents.EV_KEY, button, 1);
        file.Write(LinuxEvents.EV_SYN, 0, 0);
    }

    public static void MouseButtonUp(this EventFile file, int button) {
        file.Write(LinuxEvents.EV_KEY, button, 0);
        file.Write(LinuxEvents.EV_SYN, 0, 0);
    }
}