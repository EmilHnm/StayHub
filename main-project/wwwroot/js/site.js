// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('alpine:init', function () {
    Alpine.directive(
        "update-input",
        (el, { expression, modifiers }, { evaluate, cleanup }) => {
            const data = evaluate(expression);

            function updateInput() {
                const file = data[data.index];
                if (file) {
                    const dataTransfer = new DataTransfer();
                    dataTransfer.items.add(file);
                    el.files = dataTransfer.files;
                }
            }

            updateInput();

            cleanup(() => {
                // Clean up any event listeners or resources if needed
            });

            return {
                update() {
                    updateInput();
                },
            };
        }
    );
    Alpine.data('select', (listItem, defaultOptions) => ({
        selectOpen: false,
        selectedItem: '',
        selectableItems: [
            {
                title: 'Select an item...',
                value: '',
                disabled: true
            },
        ],
        selectableItemActive: null,
        selectId: defaultOptions,
        selectKeydownValue: '',
        selectKeydownTimeout: 1000,
        selectKeydownClearTimeout: null,
        selectDropdownPosition: 'bottom',
        selectableItemIsActive(item) {
            return this.selectableItemActive && this.selectableItemActive.value == item.value;
        },
        selectableItemActiveNext() {
            let index = this.selectableItems.indexOf(this.selectableItemActive);
            if (index < this.selectableItems.length - 1) {
                this.selectableItemActive = this.selectableItems[index + 1];
                this.selectScrollToActiveItem();
            }
        },
        selectableItemActivePrevious() {
            let index = this.selectableItems.indexOf(this.selectableItemActive);
            if (index > 0) {
                this.selectableItemActive = this.selectableItems[index - 1];
                this.selectScrollToActiveItem();
            }
        },
        selectScrollToActiveItem() {
            if (this.selectableItemActive) {
                activeElement = document.getElementById(this.selectableItemActive.value + '-' + this.selectId)
                newScrollPos = (activeElement.offsetTop + activeElement.offsetHeight) - this.$refs.selectableItemsList.offsetHeight;
                if (newScrollPos > 0) {
                    this.$refs.selectableItemsList.scrollTop = newScrollPos;
                } else {
                    this.$refs.selectableItemsList.scrollTop = 0;
                }
            }
        },
        selectKeydown(event) {
            if (event.keyCode >= 65 && event.keyCode <= 90) {

                this.selectKeydownValue += event.key;
                selectedItemBestMatch = this.selectItemsFindBestMatch();
                if (selectedItemBestMatch) {
                    if (this.selectOpen) {
                        this.selectableItemActive = selectedItemBestMatch;
                        this.selectScrollToActiveItem();
                    } else {
                        this.selectedItem = this.selectableItemActive = selectedItemBestMatch;
                    }
                }

                if (this.selectKeydownValue != '') {
                    clearTimeout(this.selectKeydownClearTimeout);
                    this.selectKeydownClearTimeout = setTimeout(() => {
                        this.selectKeydownValue = '';
                    }, this.selectKeydownTimeout);
                }
            }
        },
        selectItemsFindBestMatch() {
            typedValue = this.selectKeydownValue.toLowerCase();
            var bestMatch = null;
            var bestMatchIndex = -1;
            for (var i = 0; i < this.selectableItems.length; i++) {
                var title = this.selectableItems[i].title.toLowerCase();
                var index = title.indexOf(typedValue);
                if (index > -1 && (bestMatchIndex == -1 || index < bestMatchIndex) && !this.selectableItems[i].disabled) {
                    bestMatch = this.selectableItems[i];
                    bestMatchIndex = index;
                }
            }
            return bestMatch;
        },
        selectPositionUpdate() {
            selectDropdownBottomPos = this.$refs.selectButton.getBoundingClientRect().top + this.$refs.selectButton.offsetHeight + parseInt(window.getComputedStyle(this.$refs.selectableItemsList).maxHeight);
            if (window.innerHeight < selectDropdownBottomPos) {
                this.selectDropdownPosition = 'top';
            } else {
                this.selectDropdownPosition = 'bottom';
            }
        },
        init() {
            if (listItem && listItem.length > 0) {
                this.selectableItems = listItem;
                if (defaultOptions) {
                    this.selectedItem = this.selectableItems.find(item => item.value == defaultOptions);
                }
                this.$watch('selectOpen', () => {
                    if (!this.selectedItem) {
                        this.selectableItemActive = this.selectableItems[0];
                    } else {
                        this.selectableItemActive = this.selectedItem;
                    }
                    setTimeout(() => {
                        this.selectScrollToActiveItem();
                    }, 10);
                    this.selectPositionUpdate();
                    window.addEventListener('resize', (event) => { this.selectPositionUpdate(); });
                });

            }
        }
    }));
    Alpine.data('datepicker', () => ({
        datePickerOpen: false,
        datePickerValue: '',
        datePickerFormat: 'M d, Y',
        datePickerMonth: '',
        datePickerYear: '',
        datePickerDay: '',
        datePickerDaysInMonth: [],
        datePickerBlankDaysInMonth: [],
        datePickerMonthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        datePickerDays: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
        datePickerDayClicked(day) {
            let selectedDate = new Date(this.datePickerYear, this.datePickerMonth, day);
            this.datePickerDay = day;
            this.datePickerValue = this.datePickerFormatDate(selectedDate);
            this.datePickerIsSelectedDate(day);
            this.datePickerOpen = false;
        },
        datePickerPreviousMonth() {
            if (this.datePickerMonth == 0) {
                this.datePickerYear--;
                this.datePickerMonth = 12;
            }
            this.datePickerMonth--;
            this.datePickerCalculateDays();
        },
        datePickerNextMonth() {
            if (this.datePickerMonth == 11) {
                this.datePickerMonth = 0;
                this.datePickerYear++;
            } else {
                this.datePickerMonth++;
            }
            this.datePickerCalculateDays();
        },
        datePickerIsSelectedDate(day) {
            const d = new Date(this.datePickerYear, this.datePickerMonth, day);
            return this.datePickerValue === this.datePickerFormatDate(d) ? true : false;
        },
        datePickerIsToday(day) {
            const today = new Date();
            const d = new Date(this.datePickerYear, this.datePickerMonth, day);
            return today.toDateString() === d.toDateString() ? true : false;
        },
        datePickerCalculateDays() {
            let daysInMonth = new Date(this.datePickerYear, this.datePickerMonth + 1, 0).getDate();
            // find where to start calendar day of week
            let dayOfWeek = new Date(this.datePickerYear, this.datePickerMonth).getDay();
            let blankdaysArray = [];
            for (var i = 1; i <= dayOfWeek; i++) {
                blankdaysArray.push(i);
            }
            let daysArray = [];
            for (var i = 1; i <= daysInMonth; i++) {
                daysArray.push(i);
            }
            this.datePickerBlankDaysInMonth = blankdaysArray;
            this.datePickerDaysInMonth = daysArray;
        },
        datePickerFormatDate(date) {
            let formattedDay = this.datePickerDays[date.getDay()];
            let formattedDate = ('0' + date.getDate()).slice(-2); // appends 0 (zero) in single digit date
            let formattedMonth = this.datePickerMonthNames[date.getMonth()];
            let formattedMonthShortName = this.datePickerMonthNames[date.getMonth()].substring(0, 3);
            let formattedMonthInNumber = ('0' + (parseInt(date.getMonth()) + 1)).slice(-2);
            let formattedYear = date.getFullYear();

            if (this.datePickerFormat === 'M d, Y') {
                return `${formattedMonthShortName} ${formattedDate}, ${formattedYear}`;
            }
            if (this.datePickerFormat === 'MM-DD-YYYY') {
                return `${formattedMonthInNumber}-${formattedDate}-${formattedYear}`;
            }
            if (this.datePickerFormat === 'DD-MM-YYYY') {
                return `${formattedDate}-${formattedMonthInNumber}-${formattedYear}`;
            }
            if (this.datePickerFormat === 'YYYY-MM-DD') {
                return `${formattedYear}-${formattedMonthInNumber}-${formattedDate}`;
            }
            if (this.datePickerFormat === 'D d M, Y') {
                return `${formattedDay} ${formattedDate} ${formattedMonthShortName} ${formattedYear}`;
            }

            return `${formattedMonth} ${formattedDate}, ${formattedYear}`;
        },
        init() {
            this.currentDate = new Date();
            if (this.datePickerValue) {
                this.currentDate = new Date(Date.parse(this.datePickerValue));
            }
            this.datePickerMonth = this.currentDate.getMonth();
            this.datePickerYear = this.currentDate.getFullYear();
            this.datePickerDay = this.currentDate.getDay();
            this.datePickerValue = this.datePickerFormatDate(this.currentDate);
            this.datePickerCalculateDays();
        }
    }));
    Alpine.data('hovercard',() => ({ 
        hoverCardHovered: false,
        hoverCardDelay: 600,
        hoverCardLeaveDelay: 500,
        hoverCardTimout: null,
        hoverCardLeaveTimeout: null,
        hoverCardEnter () {
            clearTimeout(this.hoverCardLeaveTimeout);
            if(this.hoverCardHovered) return;
            clearTimeout(this.hoverCardTimout);
            this.hoverCardTimout = setTimeout(() => {
                this.hoverCardHovered = true;
                const root = this.$refs.root;
                const card = this.$refs.card;
                if(root && card) {
                    const rootRect = root.getBoundingClientRect();
                    const cardRect = card.getBoundingClientRect();
                    const top = rootRect.top + rootRect.height;
                    const left = rootRect.left + (rootRect.width / 2) - (cardRect.width / 2);
                    card.style.top = `${top}px`;
                    card.style.left = `${left}px`; 
                }
            }, this.hoverCardDelay);
        },
        hoverCardLeave () {
            clearTimeout(this.hoverCardTimout);
            if(!this.hoverCardHovered) return;
            clearTimeout(this.hoverCardLeaveTimeout);
            this.hoverCardLeaveTimeout = setTimeout(() => {
                this.hoverCardHovered = false;
            }, this.hoverCardLeaveDelay);
        },
    }));
    Alpine.data('accordion', (defaultOptions = '') => ({ 
        activeAccordion: defaultOptions, 
        setActiveAccordion(id) { 
            this.activeAccordion = (this.activeAccordion == id) ? '' : id 
        },
    }));
});