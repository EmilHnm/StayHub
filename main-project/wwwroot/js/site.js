// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('alpine:init', function () {
    Alpine.directive(
        "update-input",
        (el, {expression, modifiers}, {evaluate, cleanup}) => {
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
            if(listItem && listItem.length > 0) {
                this.selectableItems = listItem;
                if(defaultOptions) {
                    this.selectedItem  = this.selectableItems.find(item => item.value == defaultOptions); 
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
});