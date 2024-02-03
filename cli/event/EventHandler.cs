namespace RskBox {
    public class EventHandle {
        private bool isEventModeActive = false;

        public bool IsEventModeActive() {
            return isEventModeActive;
        }

        public void ActivateEventMode() {
            isEventModeActive = true;
        } public void DeactivateEventMode() {
            isEventModeActive = false;
        }
    }
}
